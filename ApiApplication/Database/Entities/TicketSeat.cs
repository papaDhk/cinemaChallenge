using System;
using System.Collections.Generic;

namespace ApiApplication.Database.Entities
{
    public class TicketSeatEntity
    {
        public Guid TicketId { get; set; }
        public TicketEntity TicketEntity { get; set; }
        public short Row { get; set; }
        public short SeatNumber { get; set; }
        public int AuditoriumId { get; set; }
        public SeatEntity SeatEntity { get; set; }
    }
}