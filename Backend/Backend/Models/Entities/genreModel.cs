using System.Text.Json.Serialization;

namespace Backend.Models.Entities
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [JsonIgnore]
        public List<MovieGenre> MovieGenres { get; set; } = [];
    }
}