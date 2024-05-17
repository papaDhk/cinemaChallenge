using ApiApplication.Database.Entities;
using ApiApplication.Services.Movies;
using ApiApplication.Services.ReservationService.Models;
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
        
        public static Seat ToSeat(this SeatEntity seatEntity, short auditoriumRowCount, short auditoriumSeatPerRow )
        {
            if (seatEntity is null)
                return null;

            return new Seat(seatEntity.Row, seatEntity.SeatNumber, seatEntity.AuditoriumId,
                (seatEntity.Row - 1) * auditoriumSeatPerRow  + seatEntity.SeatNumber );

        }
        
        public static Seat ToSeat(this SeatEntity seatEntity )
        {
            if (seatEntity is null)
                return null;

            return new Seat(seatEntity.Row, seatEntity.SeatNumber, seatEntity.AuditoriumId);

        }
        
        public static SeatEntity ToSeatEntity(this Seat seat)
        {
            if (seat is null)
                return null;

            return new SeatEntity
            {
                AuditoriumId = seat.AuditoriumId,
                Row = seat.Row,
                SeatNumber = seat.SeatNumber
            };

        }
        
        public static Ticket ToTicket(this TicketEntity ticketEntity )
        {
            if (ticketEntity is null)
                return null;

            return new Ticket
            {
                CreatedTime = ticketEntity.CreatedTime,
                Id = ticketEntity.Id,
                Movie = ticketEntity.Showtime.Movie.ToMovie(),
                Paid = ticketEntity.Paid,
                ShowtimeId = ticketEntity.ShowtimeId,
                NumberOfSeats = ticketEntity.TicketSeats.Count
            };

        }
    }
}