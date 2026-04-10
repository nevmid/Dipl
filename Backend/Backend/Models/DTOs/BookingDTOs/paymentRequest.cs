using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs.BookingDTOs
{
    public class PaymentRequest
    {
        public int BookingId { get; set; }
        public double Amount { get; set; }
        public int UserId {  get; set; }
        public string PaymentMethod { get; set; } = "card";
        public string? CardNumber { get; set; }
        public string? CardHolder { get; set; }
        public string? ExpiryDate { get; set; }
        public string? CVV { get; set; }
    }
}
