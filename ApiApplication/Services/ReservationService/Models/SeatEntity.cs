using System;
using System.Collections.Generic;

namespace ApiApplication.Database.Entities
{
    public class Seat
    {
        public Seat(short row, short seatNumber, int auditoriumId, int virtualSeatNumber)
        {
            Row = row;
            SeatNumber = seatNumber;
            AuditoriumId = auditoriumId;
            VirtualSeatNumber = virtualSeatNumber;
        }

        public Seat(short row, short seatNumber, int auditoriumId)
        {
            Row = row;
            SeatNumber = seatNumber;
            AuditoriumId = auditoriumId;
        }

        public short Row { get; private set; }
        public short SeatNumber { get; private set; }
        public int AuditoriumId { get; private set; }
        public int VirtualSeatNumber { get; private set; }
    }
}
