using System.ComponentModel.DataAnnotations;
using Backend.Infrastructure;
using Backend.Interfaces;
using Backend.Models.DTOs.UserDTOs;
using Backend.Models.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly JwtProvider _jwtProvider;
        private readonly PasswordHasher _hasher;
        private readonly IUserRepository _userRepository;

        public AuthService(
            JwtProvider jwtProvider,
            PasswordHasher hasher,
            IUserRepository userRepository)
        {
            _jwtProvider = jwtProvider;
            _hasher = hasher;
            _userRepository = userRepository;
        }
        public async Task<(UserDto user, string token)> LoginAsync(UserLoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmail(loginDto.Email.ToLower());

            if (user == null)
            {
                throw new ValidationException("Неверный email или пароль");
            }

            if (_hasher.Verify(loginDto.Password, user.PasswordHash) == false)
            {
                throw new ValidationException("Неверный email или пароль");
            }

            var roleName = await _userRepository.GetRoleNameById(user.RoleId);

            var response = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = roleName!,
                CreatedAt = user.CreatedAt,
                loyaltyAccount = user.LoyaltyAccount ?? null
            };

            var token = _jwtProvider.generateAuthToken(response);


            return (response, token);
        }

        public async Task<(UserDto user, string token)> RegisterAsync(CreateUserDto createDto)
        {
            try
            {
                if (await _userRepository.EmailExistAsync(createDto.Email.ToLower()))
                {
                    throw new ValidationException("Пользователь с таким email уже существует");
                }

                var passwordHash = _hasher.Generate(createDto.Password);

                List<string> roles = ["admin", "user"];

                if (!roles.Contains(createDto.Role.ToLower()))
                {
                    throw new ValidationException($"Роль {createDto.Role} не существует");
                }

                var roleId = await _userRepository.GetRoleIdByName(createDto.Role.ToLower());

                var user = new User
                {
                    Email = createDto.Email.ToLower(),
                    PasswordHash = passwordHash,
                    RoleId = roleId,
                    CreatedAt = DateTime.UtcNow
                };

                var createdUser = await _userRepository.CreateUserAsync(user);

                var account = new LoyaltyAccount
                {
                    UserId = createdUser.Id,
                    Balance = 0
                };

                var createdAccount = await _userRepository.CreateLoyaltyAccount(account);

                var response = new UserDto
                {
                    Id = createdUser.Id,
                    Email = createdUser.Email,
                    Role = createDto.Role.ToLower(),
                    CreatedAt = createdUser.CreatedAt,
                    loyaltyAccount = createdAccount
                };

                var token = _jwtProvider.generateAuthToken(response);


                return (response, token);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch(Exception ex)
            {
                throw new Exception("Ошибка при регистрации пользователя", ex);
            }
        }
    }
}
