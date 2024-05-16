using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication.Services.Movies
{
    public interface IMoviesService
    {
        Task<IReadOnlyCollection<Movie>> GetAllMoviesAsync(CancellationToken cancellationToken);
        Task<IReadOnlyCollection<Movie>> SearchMoviesAsync(string search, CancellationToken cancellationToken);
        Task<Movie> GetMovieByIdAsync(string id, CancellationToken cancellationToken);
    }
}