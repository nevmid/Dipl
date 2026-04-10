using Backend.Interfaces;
using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class SeatRepository : ISeatRepository
    {
        private readonly ApplicationDbContext _context;

        public SeatRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task CreateSeat(Seat seat)
        {
            await _context.Seats.AddAsync(seat);
            return;
        }

        public async Task DeleteSeats(int id)
        {
            await _context.Seats
                .Where(s => s.HallId == id)
                .ExecuteDeleteAsync();
        }

        public async Task<List<Seat>> GetSeatsByIdsAsync(List<int> seatsIds)
        {
            return await _context.Seats
                .Where(seat => seatsIds.Contains(seat.Id))
                .ToListAsync();
        }
    }
}
