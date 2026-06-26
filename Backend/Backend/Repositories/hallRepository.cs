using Backend.Interfaces;
using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class HallRepository : IHallRepository
    {
        private readonly ApplicationDbContext _context;

        public HallRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Hall> CreateHall(Hall hall)
        {
            await _context.Halls.AddAsync(hall);
            await _context.SaveChangesAsync();
            return hall;
        }

        public void DeleteHall(Hall hall)
        {
            _context.Halls.Remove(hall);
        }

        public async Task<Hall?> GetHallById(int id)
        {
            return await _context.Halls
                .Include(h => h.Seats)
                .Include(h => h.Sessions).
                FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task<List<Hall>> GetHalls()
        {
            return await _context.Halls
                .Include(h => h.Seats)
                .Include(h => h.Sessions)
                .ToListAsync();
        }

        public async Task<bool> HallIsExist(string name, int? id)
        {
            var query = _context.Halls.AsQueryable();

            query = query.Where(h => h.Name.ToLower() == name.ToLower());

            if (id.HasValue)
            {
                query = query.Where(h => h.Id != id.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> HasActiveSessions(int hallId)
        {
            return await _context.Sessions
                .AnyAsync(s => s.HallId == hallId
                    && s.StartTime > DateTime.UtcNow);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
