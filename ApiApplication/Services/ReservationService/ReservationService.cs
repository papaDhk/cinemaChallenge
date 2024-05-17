using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Database.Entities;
using ApiApplication.Database.Repositories.Abstractions;
using ApiApplication.Services.Auditorium;
using ApiApplication.Services.ReservationService.Models;
using ApiApplication.Services.Showtimes;
using MoreLinq;

namespace ApiApplication.Services.ReservationService
{
    public class ReservationService : IReservationService
    {
        private readonly ITicketsRepository _ticketsRepository;
        private readonly IAuditoriumService _auditoriumService;
        private readonly IShowtimesService _showtimesService;
        private readonly TimeSpan _reservationTimeToExpiration = TimeSpan.FromMinutes(10);

        public ReservationService(ITicketsRepository ticketsRepository, IAuditoriumService auditoriumService, IShowtimesService showtimesService)
        {
            _ticketsRepository = ticketsRepository ?? throw new ArgumentNullException(nameof(ticketsRepository));
            _auditoriumService = auditoriumService ?? throw new ArgumentNullException(nameof(auditoriumService));
            _showtimesService = showtimesService ?? throw new ArgumentNullException(nameof(showtimesService));
        }

        public async Task<Ticket> ReserveSeats(int showtimeId, int numberOfSeats, CancellationToken cancellationToken = default)
        {
            // Avoid race condition by implementing a distributed lock
            
            var availableSeats = (await GetAvailableSeats(showtimeId, cancellationToken)).ToArray();

            if (availableSeats.Length < numberOfSeats)
                throw new NotEnoughSeatsAvailableException();

            var chosenSeatsToReserve = SelectContiguousSeatToReserve(availableSeats, numberOfSeats);

            var ticketEntity = await _ticketsRepository.CreateAsync(new ShowtimeEntity
            {
                Id = showtimeId
            }, chosenSeatsToReserve.Select(s => s.ToSeatEntity()), cancellationToken);

            return ticketEntity.ToTicket();
        }

        private static IEnumerable<Seat> SelectContiguousSeatToReserve(IReadOnlyList<Seat> availableSeats, int numberOfSeatsToReserve)
        {
            var i = 0;
            if (numberOfSeatsToReserve == 1)
                return new[] { availableSeats[0] };

            var found = false;
            while (i < availableSeats.Count - numberOfSeatsToReserve && !found)
            {
                for (var j = 1; j < availableSeats.Count; j++)
                {
                    if (availableSeats[j].VirtualSeatNumber - availableSeats[j-1].VirtualSeatNumber == 1)
                    {
                        found = j - i + 1 == numberOfSeatsToReserve;
                        if(found) 
                            break;
                    }
                    else
                    {
                        i = j;
                        break;
                    }
                }
            }

            if (found)
                return availableSeats.Slice(i, numberOfSeatsToReserve);
            
            throw new NotEnoughSeatsAvailableException($"Unable to find {numberOfSeatsToReserve} contiguous free seats");
        }

        public async Task<IEnumerable<Seat>> GetAvailableSeats(int showtimeId, CancellationToken cancellationToken = default)
        {
            var auditoriumId = (await _showtimesService.GetShowtimeByIdAsync(showtimeId, cancellationToken)).AuditoriumId;
            var auditorium = await _auditoriumService.GetAuditorium(auditoriumId, cancellationToken);
            var reservedSeats = (await GetReservedSeats(showtimeId, cancellationToken))
                .Select(s => s.ToSeat(auditorium.RowsCount, auditorium.NumberOfSeatsPerRow));
            return auditorium.Seats.ExceptBy(reservedSeats, seat => seat.VirtualSeatNumber).ToArray();
        }
        
        public async Task<IEnumerable<SeatEntity>> GetReservedSeats(int showtimeId, CancellationToken cancellationToken = default)
        {
            var tickets = (await _ticketsRepository.GetEnrichedAsync(showtimeId, cancellationToken)).ToArray();

            if (!tickets.Any())
                return Enumerable.Empty<SeatEntity>();

            var paidOrNoExpiredReservedSeats = tickets.Where(t => t.Paid || !IsTicketReservationExpired(t, _reservationTimeToExpiration))
                                                            .SelectMany(t => t.TicketSeats)
                                                            .Select(ts => ts.SeatEntity);

            return paidOrNoExpiredReservedSeats;
        }

        public async Task<Ticket> ConfirmSeatReservation(Guid ticketId, CancellationToken cancellationToken = default)
        {
            var ticketEntity = await _ticketsRepository.GetAsync(ticketId, cancellationToken);
            if (ticketEntity is null)
                throw new NotFoundException($"There is not ticket corresponding to the id {ticketId}");
            if (ticketEntity.Paid)
                throw new TicketAlreadyPaidException();
            if (IsTicketReservationExpired(ticketEntity, _reservationTimeToExpiration))
                throw new SeatsReservationExpiredException();

            return (await _ticketsRepository.ConfirmPaymentAsync(ticketEntity, cancellationToken)).ToTicket();
        }

        private static bool IsTicketReservationExpired(TicketEntity ticketEntity, TimeSpan expirationDelay)
        {
            return !ticketEntity.Paid && ticketEntity.CreatedTime.Add(expirationDelay) <= DateTime.UtcNow;
        }
        
    }
}