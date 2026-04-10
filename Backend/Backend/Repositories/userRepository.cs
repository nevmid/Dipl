using System.Runtime.CompilerServices;
using Backend.Interfaces;
using Backend.Models.DTOs.UserDTOs;
using Backend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LoyaltyAccount> CreateLoyaltyAccount(LoyaltyAccount account)
        {
            await _context.LoyaltyAccounts.AddAsync(account);
            await _context.SaveChangesAsync();
            return account;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task DeleteUser(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return;
        }

        public async Task<bool> EmailExistAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users
                .Include(u => u.LoyaltyAccount)
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        
        public async Task<User?> GetUserById(int id)
        {
            return await _context.Users
                .Include(u => u.LoyaltyAccount)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<(List<User> users, int totalCount)> GetUsers(int page, int pageSize)
        {
            var query = _context.Users
                .Include(u => u.LoyaltyAccount);

            var totalCount = await query.CountAsync();

            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users , totalCount);
        }

        public async Task<List<User>> SearchUsers(string search, int limit)
        {
            return await _context.Users
                .Where(u => u.Email.Contains(search))
                .Take(limit)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
            return;
        }
    }
}
