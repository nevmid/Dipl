using Backend.Models.Entities;

namespace Backend.Interfaces
{
    public interface IBarRepository
    {
        public Task<BarCategory?> GetCategoryById(int id);
        public Task<BarItem?> GetItemById(int id);
        public Task<bool> CategoryIsExist(string name);
        public Task<bool> ItemIsExist(string name);
        public Task<BarCategory> CreateCategory(BarCategory barCategory);
        public Task<BarItem> CreateItem(BarItem barItem);
        public Task<List<BarCategory>> GetCategories();
        public Task<List<BarItem>> GetItems();
        public void DeleteCategory(BarCategory barCategory);
        public void DeleteItem(BarItem barItem);
        public Task SaveChangesAsync();
    }
}
