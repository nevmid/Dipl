using System.Text.Json.Serialization;

namespace Backend.Models.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SessionId { get; set; }
        public string Status { get; set; } = string.Empty;
        public double TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public User User { get; set; } = null!;
        public Session Session { get; set; } = null!;
        public List<BookingSeat> BookingSeats { get; set; } = [];
        public Payment? Payment { get; set; }
    }
}
