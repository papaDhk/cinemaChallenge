using ApiApplication.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using ApiApplication.Database.Repositories.Abstractions;

namespace ApiApplication.Database.Repositories
{
    public class TicketsRepository : ITicketsRepository
    {
        private readonly CinemaContext _context;

        public TicketsRepository(CinemaContext context)
        {
            _context = context;
        }

        public Task<TicketEntity> GetAsync(Guid id, CancellationToken cancel)
        {
            return _context.Tickets.FirstOrDefaultAsync(x => x.Id == id, cancel);
        }

        public async Task<IEnumerable<TicketEntity>> GetEnrichedAsync(int showtimeId, CancellationToken cancel)
        {
            return await _context.Tickets
                .Include(x => x.Showtime)
                .Include(x => x.TicketSeats).ThenInclude(ts => ts.SeatEntity)
                .Where(x => x.ShowtimeId == showtimeId)
                .ToListAsync(cancel);
        }

        public async Task<TicketEntity> CreateAsync(ShowtimeEntity showtime, IEnumerable<SeatEntity> selectedSeats, CancellationToken cancel)
        {
            var ticket = _context.Tickets.Add(new TicketEntity
            {
                ShowtimeId = showtime.Id
            });

            await _context.SaveChangesAsync(cancel);

            ticket.Entity.TicketSeats = selectedSeats.Select(s => new TicketSeatEntity
            {
                AuditoriumId = s.AuditoriumId,
                Row = s.Row,
                SeatNumber = s.SeatNumber,
                TicketId = ticket.Entity.Id,
            }).ToList();
            
            await _context.SaveChangesAsync(cancel);

            return ticket.Entity;
        }

        public async Task<TicketEntity> ConfirmPaymentAsync(TicketEntity ticket, CancellationToken cancel)
        {
            ticket.Paid = true;
            _context.Update(ticket);
            await _context.SaveChangesAsync(cancel);
            return ticket;
        }
    }
}
