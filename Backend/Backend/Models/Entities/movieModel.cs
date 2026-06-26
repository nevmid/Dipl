using System.Text.Json.Serialization;

namespace Backend.Models.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string OriginalTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Duration { get; set; }
        public int Age { get; set; }
        public string PosterUrl { get; set; } = string.Empty;
        public string TrailerUrl { get; set; } = string.Empty;
        [JsonIgnore]
        public List<Session> Sessions { get; set; } = [];
        [JsonIgnore]
        public List<MovieGenre> MovieGenres { get; set; } = [];
    }
}
