using System.Collections.Generic;
using ApiApplication.Database.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ApiApplication.Database.Repositories.Abstractions
{
    public interface IMoviesRepository
    {
        Task<MovieEntity> GetMovieByImdbIdAsync(string ImdbId, CancellationToken cancel = default);
        Task<MovieEntity> CreateMovieAsync(MovieEntity movieEntity, CancellationToken cancel = default);
    }
}