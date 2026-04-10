using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs.BookingDTOs
{
    public class ProcessPaymentDto
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public double Amount { get; set; }
        [Required]
        public string PaymentMethod { get; set; } = "card";
        public string? CardNumber { get; set; }
        public string? CardHolder { get; set; }
        public string? ExpiryDate { get; set; }
        public string? CVV { get; set; }
    }
}
