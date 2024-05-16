using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public async Task<Showtime> CreateShowtime(ShowTimeCreationParameters showTimeCreationParameters, CancellationToken cancel)
        {
            if(showTimeCreationParameters.SessionDate < DateTime.UtcNow)
                throw new ArgumentException("Cannot create a showtime in the past");
            
            var isAuditoriumAvailable = await _auditoriumService.IsAuditoriumAvailable(showTimeCreationParameters.AuditoriumId, showTimeCreationParameters.SessionDate, TimeSpan.FromHours(2), cancel);
            if (!isAuditoriumAvailable)
                throw new ArgumentException("The auditorium requested for this showtime is not available");
            //Ideally we should make sure the auditorium is available for the movie duration,
            //i.e there is no scheduled show during the session date and the session date + movie duration
            
            var movie = await _moviesService.GetMovieByIdAsync(showTimeCreationParameters.MovieImDbId, cancel);
            if (movie is null)
                throw new ArgumentException("The movie requested for this showtime is not available");
            
            var showtimeEntity = new ShowtimeEntity
            {
                SessionDate = showTimeCreationParameters.SessionDate,
                Movie = movie.ToMovieEntity(),
                AuditoriumId = 1,
            };
            
            showtimeEntity = await _showtimesRepository.CreateShowtime(showtimeEntity, cancel);

            return showtimeEntity.ToShowTime();

        }

        public async Task<IEnumerable<Showtime>> GetAllAsync(CancellationToken cancel)
        {
            var showtimeEntities = await _showtimesRepository.GetAllAsync(null, cancel);
            return showtimeEntities.Select(s => s.ToShowTime());
        }

        //Could implement this to allow users to see which seats have already been reserved
        public async Task<Showtime> GetShowtimeByIdAsync(int id, CancellationToken cancel)
        {
            var showtimeEntity = await _showtimesRepository.GetWithMoviesByIdAsync(id, cancel);
            return showtimeEntity.ToShowTime();
        }
    }
}