using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiApplication.Database.Entities
{
    public class ShowtimeEntity
    {
        public int Id { get; set; }
        public MovieEntity Movie { get; set; }
        public DateTime SessionDate { get; set; }
        public int AuditoriumId { get; set; }
        public ICollection<TicketEntity> Tickets { get; set; }
    }
}
