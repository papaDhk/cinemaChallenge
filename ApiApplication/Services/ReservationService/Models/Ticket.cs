using System;
using System.Collections.Generic;
using ApiApplication.Database.Entities;
using ApiApplication.Services.Movies;
using ApiApplication.Services.Showtimes.Models;

namespace ApiApplication.Services.ReservationService.Models
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public int ShowtimeId { get; set; }
        public DateTime SessionDateTime { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool Paid { get; set; }
        public Movie Movie { get; set; }
        public int NumberOfSeats { get; set; }
    }
}