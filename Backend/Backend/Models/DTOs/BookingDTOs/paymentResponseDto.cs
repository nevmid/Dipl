namespace Backend.Models.DTOs.BookingDTOs
{
    public class PaymentResponseDto
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
