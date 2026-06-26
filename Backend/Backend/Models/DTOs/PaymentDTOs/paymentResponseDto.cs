namespace Backend.Models.DTOs.PaymentDTOs
{
    public class PaymentResponseDto
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
