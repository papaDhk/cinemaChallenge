using System.Collections.Generic;

namespace ApiApplication.Database.Entities
{
    public class SeatEntity
    {
        public short Row { get; set; }
        public short SeatNumber { get; set; }
        public int AuditoriumId { get; set; }
        public AuditoriumEntity Auditorium { get; set; }
        public ICollection<TicketSeatEntity> TicketSeats { get; set; }
    }
}
