using System.Text.Json.Serialization;

namespace Backend.Models.Entities
{
    public class Session
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int HallId { get; set; }
        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime EndTime { get; set; } = DateTime.UtcNow;
        public double Price { get; set; }
        [JsonIgnore]
        public Movie Movie { get; set; } = null!;
        [JsonIgnore]
        public Hall Hall { get; set; } = null!;
        [JsonIgnore]
        public List<Booking> Bookings { get; set; } = [];
    }
}
