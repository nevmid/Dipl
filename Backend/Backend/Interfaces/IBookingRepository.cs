using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace Backend.Interfaces
{
    public interface IBookingRepository
    {
        public Task<List<Booking>> GetUserBookings(int id);
        public Task<bool> AreSeatsAvailableAsync(int sessionId, List<int> seatIds);
        public Task CreateAsync(Booking booking);
        public Task<Booking?> GetByIdWithDetailsAsync(int id);
        public Task<List<Booking>> GetAllAsync();
        public Task<Booking?> GetByIdAsync(int id);
        public Task UpdateAsync(Booking booking);
        public Task<bool> DeleteAsync(int id);
        public Task<Booking?> GetByTicketNumberAsync(string ticketNumber);
        public Task CreateTicketAsync(Ticket ticket);
        public Task<IDbContextTransaction> BeginTransactionAsync();
        public Task SaveChangesAsync();
    }
}
