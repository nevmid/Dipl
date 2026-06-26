using Backend.Models.DTOs.PaymentDTOs;

namespace Backend.Interfaces
{
    public interface IYookassaService
    {
        Task<(bool success, string? paymentId, string? paymentUrl, string? error)> CreatePaymentAsync(
            decimal amount, string description, int bookingId, int userId);
        Task<bool> CancelPaymentAsync(string paymentId);
        Task<(bool success, string? refundId, string? error)> CreateRefundAsync(string paymentId, decimal amount, string description);
    }
}
