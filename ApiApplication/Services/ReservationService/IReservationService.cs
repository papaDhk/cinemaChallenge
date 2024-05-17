using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApiApplication.Database.Entities;
using ApiApplication.Services.ReservationService.Models;

namespace ApiApplication.Services.ReservationService
{
    public interface IReservationService
    {
        public Task<Ticket> ReserveSeats(int showtimeId, int numberOfSeats, CancellationToken cancellationToken = default);
        public Task<IEnumerable<Seat>> GetAvailableSeats(int showtimeId, CancellationToken cancellationToken = default);
        public Task<Ticket> ConfirmSeatReservation(Guid ticketId, CancellationToken cancellationToken = default);
    }
}