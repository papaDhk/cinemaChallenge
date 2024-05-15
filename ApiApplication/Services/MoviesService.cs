using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiApplication.Database.Entities;
using ApiApplication.Dto;
using Microsoft.EntityFrameworkCore.Internal;
using ProtoDefinitions;

namespace ApiApplication.Services
{
    public class MoviesService: IMoviesService
    {
        private readonly MoviesApi.MoviesApiClient _moviesApiClient;

        public MoviesService(MoviesApi.MoviesApiClient moviesApiClient)
        {
            _moviesApiClient = moviesApiClient;
        }

        public async Task<IReadOnlyCollection<Movies>> GetAllMoviesAsync()
        {
            var responses = await _moviesApiClient.GetAllAsync(new Empty());
            responses.Data.TryUnpack<showListResponse>(out var data);
            return data.Shows.Select(ToMoviesDto).ToList();
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