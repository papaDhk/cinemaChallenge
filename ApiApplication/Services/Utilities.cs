using ApiApplication.Database.Entities;
using ApiApplication.Services.Movies;
using ApiApplication.Services.Showtimes.Models;

namespace ApiApplication.Services
{
    public static class Utilities
    {
        public static MovieEntity ToMovieEntity(this Movie movie)
        {
            if (movie is null)
                return null;
            
            return new MovieEntity
            {
                ImdbId = movie.ImdbId,
                ReleaseDate = movie.ReleaseDate,
                Stars = movie.Stars,
                Title = movie.Title
            };
        }
        
        public static Movie ToMovie(this MovieEntity movie)
        {
            if (movie is null)
                return null;
            
            return new Movie
            {
                ImdbId = movie.ImdbId,
                ReleaseDate = movie.ReleaseDate,
                Stars = movie.Stars,
                Title = movie.Title
            };
        }
        
        public static Showtime ToShowTime(this ShowtimeEntity showtime)
        {
            if (showtime is null)
                return null;

            return new Showtime
            {
                AuditoriumId = showtime.AuditoriumId,
                Movie = showtime.Movie.ToMovie(),
                Id = showtime.Id,
                SessionDate = showtime.SessionDate
            };
        }
    }
}