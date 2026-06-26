using Backend.Models.DTOs.TicketDTOs;

namespace Backend.Interfaces
{
    public interface ITicketService
    {
        Task<TicketDto> GenerateTicketAsync(int bookingId);
        Task<TicketValidationResultDto> ValidateTicketAsync(string token);
        Task<bool> MarkTicketAsUsedAsync(string token);
        Task<TicketDto?> GetTicketByBookingIdAsync(int bookingId);
    }
}
