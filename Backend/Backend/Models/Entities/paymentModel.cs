using System.Text.Json.Serialization;

namespace Backend.Models.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public double Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        [JsonIgnore]
        public Booking Booking { get; set; } = null!;
    }
}
