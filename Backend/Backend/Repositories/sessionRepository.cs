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
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Session>> GetSessions()
        {
            return await _context.Sessions.ToListAsync();
        }

        public async Task<bool> IsHallAvailable(int id, DateTime startTime, DateTime endTime)
        {
            const int preparationMinutes = 30;

            var preparationTime = TimeSpan.FromMinutes(preparationMinutes);
            var adjustedStartTime = startTime - preparationTime;
            var adjustedEndTime = endTime + preparationTime;

            return await _context.Sessions
                .Where(s => s.HallId == id
                        && s.StartTime < adjustedEndTime
                        && s.EndTime > adjustedStartTime)
                .CountAsync() == 0;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
