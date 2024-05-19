using System.Collections.Generic;
using ApiApplication.Database.Entities;

namespace ApiApplication.Services.Auditorium
{
    public class Auditorium
    {
        public int Id { get; set; }
        public IEnumerable<Seat> Seats { get; set; }
        public short RowsCount { get; set; }
        public short NumberOfSeatsPerRow { get; set; }
        public int Size => NumberOfSeatsPerRow * RowsCount;
    }
}