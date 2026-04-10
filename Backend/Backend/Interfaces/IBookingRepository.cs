using Backend.Models.Entities;

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
        public Task SaveChangesAsync();
    }
}
