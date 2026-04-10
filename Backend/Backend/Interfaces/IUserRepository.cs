using Backend.Models.Entities;

namespace Backend.Interfaces
{
    public interface IUserRepository
    {
        public Task<User> CreateUserAsync(User user);
        public Task<LoyaltyAccount> CreateLoyaltyAccount(LoyaltyAccount account);
        public Task<bool> EmailExistAsync(string email);
        public Task<User?> GetUserByEmail(string email);
        public Task<User?> GetUserById(int id);
        public Task<(List<User> users, int totalCount)> GetUsers(int page, int pageSize);
        public Task<List<User>> SearchUsers(string search, int limit);
        public Task DeleteUser(User user);
        public Task SaveChangesAsync();
    }
}
