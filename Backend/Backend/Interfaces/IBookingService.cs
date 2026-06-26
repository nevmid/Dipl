using Backend.Models.DTOs.BookingDTOs;
using Backend.Models.DTOs.PaymentDTOs;
using Backend.Models.Entities;

namespace Backend.Interfaces
{
    public interface IBookingService
    {
        public Task<BookingResponseDto?> CreateBookingAsync(int userId, CreateBookingDto dto);
        public Task<BookingResponseDto?> GetBookingAsync(int id, int? userId = null);
        public Task<List<BookingResponseDto>> GetUserBookingsAsync(int userId);
        public Task<List<BookingResponseDto>> GetAllBookingsAsync();
        public Task<bool> CancelBookingAsync(int id, int userId);
        public Task<PaymentResponseDto?> GetPaymentStatusAsync(int bookingId, int userId);
        Task<PaymentInitiationDto> InitiatePaymentAsync(int bookingId, int userId);
        Task<bool> ConfirmPaymentAndGenerateTicketAsync(int bookingId, string transactionId);
        public Task<RefundResponseDto> ProcessRefundAsync(int bookingId, string reason, int userId);
    }
}
