using System;
using System.Collections.Generic;
using ApiApplication.Database.Entities;
using ApiApplication.Services.Movies;

namespace ApiApplication.Services.Showtimes.Models
{
    public class Showtime
    {
        public int Id { get; set; }
        public Movie Movie { get; set; }
        public DateTime SessionDate { get; set; }
        public int AuditoriumId { get; set; }
        public ICollection<TicketEntity> Tickets { get; set; }
    }
}