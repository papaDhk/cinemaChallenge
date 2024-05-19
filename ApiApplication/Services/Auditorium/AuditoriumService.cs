using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Database.Repositories.Abstractions;

namespace ApiApplication.Services.Auditorium
{
    public class AuditoriumService : IAuditoriumService
    {
        private readonly IAuditoriumsRepository _auditoriumsRepository;

        public AuditoriumService(IAuditoriumsRepository auditoriumsRepository)
        {
            _auditoriumsRepository = auditoriumsRepository;
        }

        public async Task<bool> IsAuditoriumAvailable(int auditoriumId, DateTime dateTime, TimeSpan reservationDuration, CancellationToken cancellationToken = default)
        {
            var auditorium = await _auditoriumsRepository.GetAsync(auditoriumId, cancellationToken);
            return auditorium != null;
            
            //Ideally we should make sure the auditorium is available for the movie duration,
            //i.e there is no scheduled show during the session date and the session date + movie duration
        }

        public async Task<Auditorium> GetAuditorium(int auditoriumId, CancellationToken cancellationToken = default)
        {
            var auditoriumEntity = await _auditoriumsRepository.GetAsync(auditoriumId, cancellationToken);

            if (auditoriumEntity is null)
                throw new NotFoundException("The requested auditorium does not exist");
            
            var seatEntities = auditoriumEntity.Seats;
            var rowsCount = seatEntities.Max(s => s.Row);
            var numberOfSeatsPerRow = seatEntities.Max(s => s.SeatNumber);
            
            return new Auditorium
            {
                Id = auditoriumEntity.Id,
                RowsCount = rowsCount,
                NumberOfSeatsPerRow = numberOfSeatsPerRow,
                Seats = seatEntities.Select(s => s.ToSeat(numberOfSeatsPerRow))
            };
        }

        public async Task<IEnumerable<Auditorium>> GetAllAuditoriums(CancellationToken cancellationToken = default)
        {
            return (await _auditoriumsRepository.GetAllAsync(cancellationToken)).Select(a => new  Auditorium
            {
                Id = a.Id,
                RowsCount = a.Seats.Max(s => s.Row),
                NumberOfSeatsPerRow = a.Seats.Max(s => s.SeatNumber)
            });

        }
    }
    
}