using Backend.Interfaces;
using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Booking>> GetUserBookings(int id)
        {
            return await _context.Bookings
                .Include(b => b.Session)
                .Where(b => b.UserId == id)
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Session)
                .ThenInclude(s => s!.Movie)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Booking?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Session)
                .ThenInclude(s => s!.Movie)
                .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
                .Include(b => b.Payment)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<Booking>> GetUserBookingsAsync(int userId)
        {
            return await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Session)
                .ThenInclude(s => s!.Movie)
                .Include(b => b.BookingSeats)
                .ThenInclude(bs => bs.Seat)
                .Include(b => b.Payment)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Session)
                .ThenInclude(s => s!.Movie)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task CreateAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
            return;
        }

        public async Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var booking = await GetByIdAsync(id);
            if (booking == null)
                return false;

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AreSeatsAvailableAsync(int sessionId, List<int> seatIds)
        {
            var bookedSeats = await _context.BookingSeats
                .Where(bs => bs.Booking.SessionId == sessionId && seatIds.Contains(bs.SeatId))
                .Select(bs => bs.SeatId)
                .ToListAsync();

            return bookedSeats.Count == 0;
        }

        public async Task<List<Booking>> GetBookingsForSeatsAsync(int sessionId, List<int> seatIds)
        {
            return await _context.Bookings
                .Where(b => b.SessionId == sessionId)
                .Where(b => b.BookingSeats.Any(bs => seatIds.Contains(bs.SeatId)))
                .Include(b => b.BookingSeats)
                .ToListAsync();
        }
    }
}
