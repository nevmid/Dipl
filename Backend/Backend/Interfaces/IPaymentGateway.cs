using Backend.Models.DTOs.BookingDTOs;

namespace Backend.Interfaces
{
    public interface IPaymentGateway
    {
        public Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest paymentRequest);
    }
}
