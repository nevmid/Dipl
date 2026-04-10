using Backend.Interfaces;
using Backend.Models.DTOs.BookingDTOs;

namespace Backend.Services
{
    public class PaymentGateway : IPaymentGateway
    {
        public Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest paymentRequest)
        {
            throw new NotImplementedException();
        }
    }
}
