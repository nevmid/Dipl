using Backend.Models.DTOs.SessionDTOs;
using Backend.Models.Entities;

namespace Backend.Interfaces
{
    public interface ISessionService
    {
        public Task<sessionsResponseDto?> GetInfoAboutSession(int id);
        public Task<List<sessionsResponseDto>> GetAll(DateTime? date, int? hallId);
        public Task<Session> CreateSession(CreateSessionDto dto);
        public Task<Session?> UpdateSession(int id, UpdateSessionDto dto);
    }
}
