using System.Text.Json.Serialization;

namespace Backend.Models.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public double Amount { get; set; }
        public int StatusId { get; set; }
        public string? TransactionId { get; set; }
        public string? PaymentUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Status Status { get; set; } = null!;
        [JsonIgnore]
        public Booking Booking { get; set; } = null!;
    }
}
