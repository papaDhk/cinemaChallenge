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

        public MoviesService(MoviesApi.MoviesApiClient moviesApiClient, IDistributedCache distributedCache, ILogger<MoviesService> logger)
        {
            _moviesApiClient = moviesApiClient;
            _distributedCache = distributedCache;
            _logger = logger;
        }

        public async Task<IReadOnlyCollection<Movies>> GetAllMoviesAsync()
        {
            var movies = await ExecuteAndCacheAsync(async () => {
                var responses = await _moviesApiClient.GetAllAsync(new Empty());
                responses.Data.TryUnpack<showListResponse>(out var data);
                return data.Shows.Select(ToMoviesDto).ToList();
            }, nameof(GetAllMoviesAsync));

            return movies;
        }
        
        private async Task<IReadOnlyCollection<Movies>> ExecuteAndCacheAsync(Func<Task<IEnumerable<Movies>>> operation, string cacheKey)
        {
            IEnumerable<Movies> movies = new List<Movies>();
            try
            {
                movies = await operation();
                var moviesJson = JsonSerializer.Serialize(movies);
                await _distributedCache.SetStringAsync(cacheKey, moviesJson, CancellationToken.None);
            }
            catch (RpcException ex)
            {
                var cache = await _distributedCache.GetStringAsync(cacheKey);
                if (cache != null)
                    movies = JsonSerializer.Deserialize<IEnumerable<Movies>>(cache);

                _logger.LogError("Not able to have a successful response from movies API", ex);
            }
            return movies.ToList();
        }
        
        public async Task<IReadOnlyCollection<Movies>> SearchMoviesAsync(string search)
        {
            return await ExecuteAndCacheAsync(async () => {
                var responses = await _moviesApiClient.SearchAsync(new SearchRequest{Text = search});
                responses.Data.TryUnpack<showListResponse>(out var data);
                return data.Shows.Select(ToMoviesDto).ToList();
            }, nameof(SearchMoviesAsync));

        }

        public async Task<Movies> GetMovieByIdAsync(string id)
        {
            var movies= await ExecuteAndCacheAsync(async () => {
                var responses = await _moviesApiClient.GetByIdAsync(new IdRequest{Id = id});
                responses.Data.TryUnpack<showResponse>(out var data);
                return new[] { ToMoviesDto(data) };
            }, nameof(GetMovieByIdAsync));
            return movies.FirstOrDefault();
        }

        private static Movies ToMoviesDto(showResponse showResponse)
        {
            return showResponse is null
                ? null
                : new Movies
                {
                    ImDbId = showResponse.Id,
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