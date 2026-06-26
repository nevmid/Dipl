using Backend.Models.DTOs.PaymentDTOs;
using Backend.Models.Entities;

namespace Backend.Models.DTOs.BookingDTOs
{
    public class BookingResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SessionId { get; set; }
        public string Status { get; set; } = string.Empty;
        public double TotalAmount { get; set; }
        public double FinalAmount { get; set; }
        public int BonusUsed { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<int> SeatIds { get; set; } = new();
        public List<string> SeatsFormatted { get; set; } = new();
        public PaymentResponseDto? Payment { get; set; } 
        public Ticket? Ticket { get; set; }
        public Movie? Movie { get; set; }
        public Session? Session { get; set; }
        public Hall? Hall { get; set; }
    }
}
