using System.Collections.Generic;
using ApiApplication.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;
using ApiApplication.Database.Repositories.Abstractions;

namespace ApiApplication.Database.Repositories
{
    public class MoviesRepository : IMoviesRepository
    {
        private readonly CinemaContext _context;

        public MoviesRepository(CinemaContext context)
        {
            _context = context;
        }

        public async Task<MovieEntity> GetMovieByImdbIdAsync(string ImdbId, CancellationToken cancel= default)
        {
            return await _context.Movies
                .FirstOrDefaultAsync(x => x.ImdbId == ImdbId, cancel);
        }

        public async Task<MovieEntity> CreateMovieAsync(MovieEntity movieEntity, CancellationToken cancel = default)
        {
            var movie = await _context.Movies.AddAsync(movieEntity, cancel);
            await _context.SaveChangesAsync(cancel);
            return movie.Entity;
        }
    }
}
