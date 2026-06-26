//using Backend.Interfaces;
//using Backend.Models.Entities;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.ChangeTracking;

//namespace Backend.Repositories
//{
//    public class BarRepository : IBarRepository
//    {
//        private readonly ApplicationDbContext _context;

//        public BarRepository(ApplicationDbContext context)
//        {
//            _context = context;
//        }
//        public async Task<bool> CategoryIsExist(string name)
//        {
//            return await _context.BarCategories
//                .AnyAsync(br => br.Name == name);
//        }

//        public async Task<BarCategory> CreateCategory(BarCategory barCategory)
//        {
//            await _context.BarCategories.AddAsync(barCategory);
//            return barCategory;
//        }

//        public async Task<BarItem> CreateItem(BarItem barItem)
//        {
//            await _context.BarItems.AddAsync(barItem);
//            return barItem;
//        }

//        public void DeleteCategory(BarCategory barCategory)
//        {
//            _context.BarCategories.Remove(barCategory);
//        }

//        public void DeleteItem(BarItem barItem)
//        {
//            _context.BarItems.Remove(barItem);
//        }

//        public async Task<List<BarCategory>> GetCategories()
//        {
//            return await _context.BarCategories
//                .ToListAsync();
//        }

//        public async Task<BarCategory?> GetCategoryById(int id)
//        {
//            return await _context.BarCategories
//                .FirstOrDefaultAsync(bc => bc.Id == id);
//        }

//        public async Task<BarItem?> GetItemById(int id)
//        {
//            return await _context.BarItems
//                .Include(ba => ba.Category)
//                .FirstOrDefaultAsync(bc => bc.Id == id);
//        }

//        public async Task<List<BarItem>> GetItems()
//        {
//            return await _context.BarItems
//                .Include(ba => ba.Category)
//                .ToListAsync();
//        }

//        public async Task<bool> ItemIsExist(string name)
//        {
//            return await _context.BarItems
//                .AllAsync(ba => ba.Name == name);
//        }

//        public async Task SaveChangesAsync()
//        {
//            await _context.SaveChangesAsync();
//        }
//    }
//}
