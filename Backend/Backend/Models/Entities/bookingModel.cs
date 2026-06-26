using System.Text.Json.Serialization;

namespace Backend.Models.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SessionId { get; set; }
        public int StatusId { get; set; }
        public double TotalAmount { get; set; }
        public double FinalAmount { get; set; }
        public int BonusUsed { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public User User { get; set; } = null!;
        public Status Status { get; set; } = null!;
        public Session Session { get; set; } = null!;
        public List<BookingSeat> BookingSeats { get; set; } = [];
        public Payment? Payment { get; set; }
        public Ticket? Ticket {  get; set; }
    }
}
