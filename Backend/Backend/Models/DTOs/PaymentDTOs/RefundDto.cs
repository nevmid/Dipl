namespace Backend.Models.DTOs.PaymentDTOs
{
    public class RefundRequestDto
    {
        public int BookingId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class RefundResponseDto
    {
        public int RefundId { get; set; }
        public string RefundTransactionId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? ErrorMessage { get; set; }
    }
}