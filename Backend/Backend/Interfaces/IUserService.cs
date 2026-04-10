using Backend.Models.DTOs.UserDTOs;
using Backend.Models.Entities;
using Microsoft.AspNetCore.Identity.Data;

namespace Backend.Interfaces
{
    public interface IUserService
    {
        public Task<UserDto?> GetProfileAsync(int id);
        public Task<(List<UserDto>, int totalCount)> GetUsersAsync(int page, int pageSize);
        public Task<List<UserDto>> SearchUsers(string query, int limit);
        public Task<UserDto?> DeleteUser(int  id);
        public Task<bool> ChangeRole(int id, ChangeRoleDto changeRoleDto);
        public Task<bool> ChangePassword(int id, ChangePasswordDto changePasswordDto);
        public Task<bool> ChangeEmail(int id, ChangeEmailDto changeEmailDto);
        public Task<List<Booking>> GetUserBookingsAsync(int id);
        public Task InitiatePasswordReset(ForgotPasswordDto forgotPasswordDto);
        public Task<bool> ResetPassword(ResetPasswordDto resetPasswordDto);
    }
}
