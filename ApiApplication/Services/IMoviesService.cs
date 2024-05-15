using System.Collections.Generic;
using System.Threading.Tasks;
using ApiApplication.Dto;

namespace ApiApplication.Services
{
    public interface IMoviesService
    {
        Task<IReadOnlyCollection<Movies>> GetAllMoviesAsync();
        Task<IReadOnlyCollection<Movies>> SearchMoviesAsync(string search);
        Task<Movies> GetMovieByIdAsync(string id);
    }
}