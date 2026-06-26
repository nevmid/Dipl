namespace Backend.Models.DTOs.PaymentDTOs
{
    public class PaymentInitiationDto
    {
        public bool Success { get; set; }
        public int? PaymentId { get; set; }
        public string? PaymentUrl { get; set; }
        public string? Error { get; set; }
    }
}
