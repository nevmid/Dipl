using Backend.Models.Entities;

namespace Backend.Interfaces
{
    public interface IHallRepository
    {
        public Task<Hall?> GetHallById(int id);
        public Task<bool> HallIsExist(string name);
        public Task<List<Hall>> GetHalls();
        public Task<Hall> CreateHall(Hall hall);
        public void DeleteHall(Hall hall);
        public Task SaveChangesAsync();
    }
}
