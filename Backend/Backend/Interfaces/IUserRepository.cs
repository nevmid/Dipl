using Backend.Models.DTOs.UserDTOs;
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
        public Task<(List<UserDto> users, int totalCount)> GetUsers(int page, int pageSize);
        public Task<List<UserDto>> SearchUsers(string search, int limit);
        public Task DeleteUser(User user);
        public Task SaveChangesAsync();
        public Task<int> GetRoleIdByName(string roleName);
        public Task<string?> GetRoleNameById(int roleId);
    }
}
