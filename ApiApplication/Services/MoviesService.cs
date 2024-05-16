using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Dto;
using Grpc.Core;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using ProtoDefinitions;

namespace ApiApplication.Services
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
            return await ExecuteAndCacheAsync(async () => {
                var responses = await _moviesApiClient.GetAllAsync(new Empty());
                responses.Data.TryUnpack<showListResponse>(out var data);
                return data.Shows.Select(ToMoviesDto).ToList();
            }, nameof(GetAllMoviesAsync));
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
            var responses = await _moviesApiClient.SearchAsync(new SearchRequest{Text = search});
            responses.Data.TryUnpack<showListResponse>(out var data);
            return data.Shows.Select(ToMoviesDto).ToList();
        }

        public async Task<Movies> GetMovieByIdAsync(string id)
        {
            var response = await _moviesApiClient.GetByIdAsync(new IdRequest{Id = id});
            response.Data.TryUnpack<showListResponse>(out var data);
            return ToMoviesDto(data.Shows.FirstOrDefault());
        }

        private Movies ToMoviesDto(showResponse showResponse)
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