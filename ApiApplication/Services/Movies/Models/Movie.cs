using System;

namespace ApiApplication.Services.Movies
{
    public class Movie
    {
        public string ImdbId { get; set; }
        public string Title { get; set; }
        public string Stars { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}