using Backend.Models.DTOs.SessionDTOs;
using Backend.Models.Entities;

namespace Backend.Interfaces
{
    public interface ISessionService
    {
        public Task<Session?> GetInfoAboutSession(int id);
        public Task<List<Session>> GetAll();
        public Task<Session> CreateSession(CreateSessionDto dto);
        public Task<Session?> UpdateSession(int id, UpdateSessionDto dto);
    }
}
