using System;

namespace ApiApplication.Services.Showtimes.Models
{
    public class ShowTimeCreationParameters
    {
        public string MovieImDbId { get; set; }
        public DateTime SessionDate { get; set; }
        public int AuditoriumId { get; set; }
    }
}