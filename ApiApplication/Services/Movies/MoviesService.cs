using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Database.Entities;
using ApiApplication.Database.Repositories.Abstractions;
using Grpc.Core;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using ProtoDefinitions;

namespace ApiApplication.Services.Movies
{
    public class MoviesService: IMoviesService
    {
        private readonly MoviesApi.MoviesApiClient _moviesApiClient;
        private readonly IDistributedCache _distributedCache;
        private readonly IMoviesRepository _moviesRepository;
        private readonly ILogger<MoviesService> _logger;
        private const string GetAllMoviesCacheKey = "GetAllMovies";
        private const string GeMovieByIdCacheKeyFormat = "GeMovieById-{0}";
        private const string SearchMovieCacheKeyFormat = "SearchMovieById-{0}";

        public MoviesService(MoviesApi.MoviesApiClient moviesApiClient, IDistributedCache distributedCache, ILogger<MoviesService> logger, IMoviesRepository moviesRepository)
        {
            _moviesApiClient = moviesApiClient ?? throw new ArgumentNullException(nameof(moviesApiClient));
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _moviesRepository = moviesRepository ?? throw new ArgumentNullException(nameof(moviesRepository));
        }

        public async Task<IReadOnlyCollection<Movie>> GetAllMoviesAsync(CancellationToken cancellationToken)
        {
            var movies = await ExecuteAndCacheAsync(async () => {
                var responses = await _moviesApiClient.GetAllAsync(new Empty());
                responses.Data.TryUnpack<showListResponse>(out var data);
                return data.Shows.Select(ToMoviesDto).ToList();
            }, GetAllMoviesCacheKey);

            return movies;
        }
        
        private async Task<IReadOnlyCollection<Movie>> ExecuteAndCacheAsync(Func<Task<IEnumerable<Movie>>> operation, string cacheKey)
        {
            IEnumerable<Movie> movies = new List<Movie>();
            try
            {
                movies = await operation();
                var moviesJson = JsonSerializer.Serialize(movies);
                await _distributedCache.SetStringAsync(cacheKey, moviesJson, CancellationToken.None);
                _logger.LogInformation("Movies api is working well");
            }
            catch (RpcException ex)
            {
                var cache = await _distributedCache.GetStringAsync(cacheKey);
                if (cache != null)
                    movies = JsonSerializer.Deserialize<IEnumerable<Movie>>(cache);
                else
                {
                    throw new MoviesServiceNotAvailableException();
                }

                _logger.LogWarning("Movies cache is being used");
            }
            return movies.ToList();
        }
        
        public async Task<IReadOnlyCollection<Movie>> SearchMoviesAsync(string search, CancellationToken cancellationToken)
        {
            return await ExecuteAndCacheAsync(async () => {
                var responses = await _moviesApiClient.SearchAsync(new SearchRequest{Text = search});
                responses.Data.TryUnpack<showListResponse>(out var data);
                return data.Shows.Select(ToMoviesDto).ToList();
            }, string.Format(SearchMovieCacheKeyFormat, search));
        }

        public async Task<Movie> GetMovieByIdAsync(string id, CancellationToken cancellationToken)
        {
            //Check if the movie already exists i our database
            var movieEntity = await _moviesRepository.GetMovieByImdbIdAsync(id, cancellationToken);
            if (movieEntity != null)
                return movieEntity.ToMovie();
            
            //If it does not exists in database, we fetch it from the movies service
            Movie externalMovie = null;
            try
            {
                var movies= await ExecuteAndCacheAsync(async () => {
                    var responses = await _moviesApiClient.GetByIdAsync(new IdRequest{Id = id});
                    responses.Data.TryUnpack<showResponse>(out var data);
                    return new[] { ToMoviesDto(data) };
                }, string.Format(GeMovieByIdCacheKeyFormat, id));
                externalMovie = movies.FirstOrDefault();
            }
            catch (MoviesServiceNotAvailableException)
            {
                //This is to try using the cache of GetAllMoviesAsync request
                var movies = await GetAllMoviesAsync(cancellationToken);

                externalMovie = movies.FirstOrDefault(m =>
                    string.Compare(m.ImdbId, id, StringComparison.InvariantCultureIgnoreCase) == 0);
            }

            if (externalMovie is null)
                throw new NotFoundException("The movie requested for this showtime is not found");
            
            movieEntity = await _moviesRepository.CreateMovieAsync(externalMovie.ToMovieEntity(), cancellationToken);

            return movieEntity.ToMovie();
        }

        private static Movie ToMoviesDto(showResponse showResponse)
        {
            return showResponse is null
                ? null
                : new Movie
                {
                    ImdbId = showResponse.Id,
                    Stars = showResponse.Crew,
                    Title = showResponse.Title,
                };
        }
    }
}