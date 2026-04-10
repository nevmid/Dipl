using System.Text.Json.Serialization;

namespace Backend.Models.Entities
{
    public class BookingSeat
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int SeatId { get; set; }
        [JsonIgnore]
        public Booking Booking { get; set; } = null!;
        [JsonIgnore]
        public Seat Seat { get; set; } = null!;
    }
}
