namespace Backend.Models.DTOs.BookingDTOs
{
    public class BookingResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SessionId { get; set; }
        public string Status { get; set; } = string.Empty;
        public double TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<int> SeatIds { get; set; } = new();
        public PaymentResponseDto? Payment { get; set; } 
    }
}
