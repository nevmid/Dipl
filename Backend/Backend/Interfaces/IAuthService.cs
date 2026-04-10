using Backend.Models.DTOs.UserDTOs;
using Backend.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Interfaces
{
    public interface IAuthService
    {
        public Task<(UserDto user, string token)> RegisterAsync(CreateUserDto request);
        public Task<(UserDto user, string token)> LoginAsync(UserLoginDto request);

    }
}
