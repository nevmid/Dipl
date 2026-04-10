using Backend.Models.Entities;

namespace Backend.Interfaces
{
    public interface ISessionRepository
    {
        public Task<bool> IsHallAvailable(int id, DateTime startTime, DateTime endTime);
        public Task<Session> CreateSession(Session session);
        public Task<Session?> GetSessionById(int id);
        public Task SaveChangesAsync();
        public Task<List<Session>> GetSessions();
    }
}
