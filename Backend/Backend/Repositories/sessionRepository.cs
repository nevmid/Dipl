using Backend.Interfaces;
using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly ApplicationDbContext _context;

        public SessionRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Session> CreateSession(Session session)
        {
            await _context.Sessions.AddAsync(session);
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task<Session?> GetSessionById(int id)
        {
            return await _context.Sessions
                .Include(s => s.Movie)
                .Include(s => s.Hall)
                .Include(s => s.Bookings)
                    .ThenInclude(b => b.Status)
                .Include(s => s.Bookings)
                    .ThenInclude(b => b.BookingSeats)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Session>> GetSessions()
        {
            return await _context.Sessions
                .Include(s => s.Hall)
                .Include(s => s.Movie)
                .ToListAsync();
        }

        public async Task<bool> IsHallAvailable(int id, DateTime startTime, DateTime endTime, int? excludeSessionId)
        {
            const int preparationMinutes = 30;

            var preparationTime = TimeSpan.FromMinutes(preparationMinutes);
            var adjustedStartTime = startTime - preparationTime;
            var adjustedEndTime = endTime + preparationTime;

            var query = _context.Sessions
                .Where(s => s.HallId == id
                    && s.StartTime < adjustedEndTime
                    && s.EndTime > adjustedStartTime);

            if (excludeSessionId.HasValue)
            {
                query = query.Where(s => s.Id != excludeSessionId.Value);
            }

            return await query.CountAsync() == 0;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
