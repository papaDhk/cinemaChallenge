using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiApplication.Services.Movies
{
    public interface IMoviesService
    {
        Task<IReadOnlyCollection<Movies>> GetAllMoviesAsync();
        Task<IReadOnlyCollection<Movies>> SearchMoviesAsync(string search);
        Task<Movies> GetMovieByIdAsync(string id);
    }
}