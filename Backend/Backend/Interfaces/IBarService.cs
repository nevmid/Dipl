using Backend.Models.DTOs.BarDTOs;
using Backend.Models.Entities;

namespace Backend.Interfaces
{
    public interface IBarService
    {
        public Task<List<BarCategory>> GetGategories();
        public Task<BarCategory> CreateCategory(string  categoryName);
        public Task<BarCategory?> UpdateCategory(int id, string name);
        public Task<bool> DeleteCategory(int id);
        public Task<List<BarItem>> GetBarItems();
        public Task<BarItem> CreateBarItem(barItemDto dto);
        public Task<BarItem?> UpdateBarItem(int id, barItemDto dto);
        public Task<bool> DeleteBarItem(int id);
    }
}
