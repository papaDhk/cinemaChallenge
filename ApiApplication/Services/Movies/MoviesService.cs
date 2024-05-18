using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly ILogger<MoviesService> _logger;
        private const string GetAllMoviesCacheKey = "GetAllMovies";
        private const string GeMovieByIdCacheKeyFormat = "GeMovieById-{0}";
        private const string SearchMovieCacheKeyFormat = "SearchMovieById-{0}";

        public MoviesService(MoviesApi.MoviesApiClient moviesApiClient, IDistributedCache distributedCache, ILogger<MoviesService> logger)
        {
            _moviesApiClient = moviesApiClient;
            _distributedCache = distributedCache;
            _logger = logger;
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
            var movies= await ExecuteAndCacheAsync(async () => {
                var responses = await _moviesApiClient.GetByIdAsync(new IdRequest{Id = id});
                responses.Data.TryUnpack<showResponse>(out var data);
                return new[] { ToMoviesDto(data) };
            }, string.Format(GeMovieByIdCacheKeyFormat, id));
            return movies.FirstOrDefault();
        }

        private static Movie ToMoviesDto(showResponse showResponse)
        {
            return showResponse is null
                ? null
                : new Movie
                {
                    ImdbId = showResponse.Id,
                    Stars = showResponse.Crew,
                    FullTitle = showResponse.FullTitle,
                    Title = showResponse.Title,
                    Image = showResponse.Image,
                    ImDbRating = showResponse.ImDbRating,
                    ImDbRatingCount = showResponse.ImDbRatingCount,
                    Rank = showResponse.Rank,
                    Year = showResponse.Year,
                };
        }
    }
}