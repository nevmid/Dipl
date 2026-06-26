using System.Text.Json.Serialization;

namespace Backend.Models.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string JwtToken { get; set; } = string.Empty;
        public bool IsUsed { get; set; }
        public int StatusId { get; set; }
        public DateTime GeneratedAt { get; set; }
        public Status Status { get; set; } = null!;
        [JsonIgnore]
        public Booking? Booking { get; set; }
    }
}
