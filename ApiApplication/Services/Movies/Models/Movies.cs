using System;

namespace ApiApplication.Services.Movies
{
    public class Movies
    {
        public string ImDbId { get; set; }
        public string Rank { get; set; }
        public string Title { get; set; }
        public string FullTitle { get; set; }
        public string Year { get; set; }
        public string Image { get; set; }
        public string Stars { get; set; }
        public string ImDbRating { get; set; }
        public string ImDbRatingCount { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}