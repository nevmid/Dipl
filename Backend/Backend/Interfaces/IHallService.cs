using Backend.Models.DTOs.HallDTOs;
using Backend.Models.Entities;

namespace Backend.Interfaces
{
    public interface IHallService
    {
        public Task<List<ResponseHallDto>> GetHalls();
        public Task<Hall?> GetHallById(int id);
        public Task<Hall?> UpdateHall(int id, RequestHallDto dto);
        public Task<bool> DeleteHall(int id);
        public Task<Hall> CreateHall(RequestHallDto dto);
    }
}
