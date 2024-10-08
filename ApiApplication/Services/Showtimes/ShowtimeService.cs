using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Database.Entities;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Services.Auditorium;
using ApiApplication.Services.Movies;
using ApiApplication.Services.Showtimes.Models;

namespace ApiApplication.Services.Showtimes
{
    public class ShowtimeService : IShowtimesService
    {
        private readonly IShowtimesRepository _showtimesRepository;
        private readonly IMoviesService _moviesService;
        private readonly IAuditoriumService _auditoriumService;

        public ShowtimeService(IShowtimesRepository showtimesRepository, IMoviesService moviesService, IAuditoriumService auditoriumService)
        {
            _showtimesRepository = showtimesRepository ?? throw new ArgumentNullException(nameof(showtimesRepository));
            _moviesService = moviesService ?? throw new ArgumentNullException(nameof(moviesService));
            _auditoriumService = auditoriumService ?? throw new ArgumentNullException(nameof(auditoriumService));
        }

        public async Task<Showtime> CreateShowtime(ShowTimeCreationParameters showTimeCreationParameters, CancellationToken cancel = default)
        {
            if(showTimeCreationParameters.SessionDate < DateTime.UtcNow)
                throw new ShowtimeCreationException("Cannot create a showtime in the past");
            
            var isAuditoriumAvailable = await _auditoriumService.IsAuditoriumAvailable(showTimeCreationParameters.AuditoriumId, showTimeCreationParameters.SessionDate, TimeSpan.FromHours(2), cancel);
            if (!isAuditoriumAvailable)
                throw new AuditoriumNotAvailableException("The auditorium requested for this showtime is not available");
            //Ideally we should make sure the auditorium is available for the movie duration,
            //i.e there is no scheduled show during the session date and the session date + movie duration
            
            var movie = await _moviesService.GetMovieByIdAsync(showTimeCreationParameters.MovieImDbId, cancel);
            if (movie is null)
                throw new NotFoundException("The movie requested for this showtime is not found");
            
            var showtimeEntity = new ShowtimeEntity
            {
                SessionDate = showTimeCreationParameters.SessionDate,
                AuditoriumId = 1,
                MovieId = movie.Id
            };
            
            showtimeEntity = await _showtimesRepository.CreateShowtime(showtimeEntity, cancel);
            showtimeEntity.Movie = movie.ToMovieEntity();

            return showtimeEntity.ToShowTime();

        }

        public async Task<IEnumerable<Showtime>> GetAllAsync(CancellationToken cancel = default)
        {
            var showtimeEntities = await _showtimesRepository.GetAllAsync(null, cancel);
            return showtimeEntities.Select(s => s.ToShowTime());
        }

        public async Task<Showtime> GetShowtimeByIdAsync(int id, CancellationToken cancel = default)
        {
            var showtimeEntity = await _showtimesRepository.GetWithMoviesByIdAsync(id, cancel);
            return showtimeEntity.ToShowTime();
        }
    }
}