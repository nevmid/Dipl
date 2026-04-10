using System.ComponentModel.DataAnnotations;
using Backend.Infrastructure;
using Backend.Interfaces;
using Backend.Models.DTOs.UserDTOs;
using Backend.Models.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Backend.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher _hasher;
        private readonly IBookingRepository _bookingRepository;
        private readonly JwtProvider _jwtProvider;
        private readonly IEmailService _emailService;

        public UserService(
            IUserRepository userRepository,
            PasswordHasher hasher,
            IBookingRepository bookingRepository,
            JwtProvider jwtProvider,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _hasher = hasher;
            _bookingRepository = bookingRepository;
            _jwtProvider = jwtProvider;
            _emailService = emailService;
        }

        public async Task<bool> ChangeEmail(int id, ChangeEmailDto changeEmailDto)
        {
            try
            {
                var user = await _userRepository.GetUserById(id);

                if (user == null)
                {
                    return false;
                }

                if (!_hasher.Verify(changeEmailDto.Password, user.PasswordHash))
                {
                    throw new ValidationException("Неверный пароль");
                }

                if (await _userRepository.EmailExistAsync(changeEmailDto.Email.ToLower()))
                {
                    throw new ValidationException("Пользователь с таким email уже существует");
                }

                user.Email = changeEmailDto.Email;
                await _userRepository.SaveChangesAsync();

                return true;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> ChangePassword(int id, ChangePasswordDto changePasswordDto)
        {
            try
            {
                var user = await _userRepository.GetUserById(id);

                if (user == null)
                {
                    return false;
                }

                if(!_hasher.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
                {
                    throw new ValidationException("Неверный пароль");
                }

                user.PasswordHash = _hasher.Generate(changePasswordDto.NewPassword);

                await _userRepository.SaveChangesAsync();
                return true;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> ChangeRole(int id, ChangeRoleDto changeRoleDto)
        {
            try
            {
                var user = await _userRepository.GetUserById(id);

                if (user == null)
                {
                    return false;
                }

                List<string> roles = ["admin", "user"];

                if(!roles.Contains(changeRoleDto.Role))
                {
                    throw new ValidationException($"Роль {changeRoleDto.Role} не существует");
                }

                if (user.Role == changeRoleDto.Role)
                {
                    return true;
                }

                user.Role = changeRoleDto.Role;

                await _userRepository.SaveChangesAsync();
                return true;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UserDto?> DeleteUser(int id)
        {
            try
            {
                var user = await _userRepository.GetUserById(id);

                if (user == null)
                {
                    return null;
                }

                await _userRepository.DeleteUser(user);

                var respone = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt,
                    loyaltyAccount = user.LoyaltyAccount
                };

                return respone;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UserDto?> GetProfileAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetUserById(id);

                if (user == null)
                {
                    return null;
                }

                var respone = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt,
                    loyaltyAccount = user.LoyaltyAccount
                };

                return respone;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Booking>> GetUserBookingsAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetUserById(id);

                if(user == null)
                {
                    return [];
                }

                var bookings = await _bookingRepository.GetUserBookings(id);

                return bookings;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(List<UserDto>, int totalCount)> GetUsersAsync(int page, int pageSize)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 25;

                var (users, totalCount) = await _userRepository.GetUsers(page, pageSize);

                List<UserDto> response = users.Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                    loyaltyAccount = u.LoyaltyAccount
                }).ToList();

                return (response, totalCount);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task InitiatePasswordReset(ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                var user = await _userRepository.GetUserByEmail(forgotPasswordDto.Email);

                if (user == null)
                {
                    return;
                }

                var resetToken = _jwtProvider.generateResetPasswordToken(user);

                await _emailService.SendPasswordResetEmail(forgotPasswordDto.Email, resetToken);

                return;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var(isValid, userId, email, type) = _jwtProvider.ValidateToken(resetPasswordDto.Token);

                if(!isValid || type != "reset_password")
                    return false;

                var user = await _userRepository.GetUserById(userId);

                if (user == null || user.Email != email)
                    return false;

                user.PasswordHash = _hasher.Generate(resetPasswordDto.NewPassword);
                await _userRepository.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<UserDto>> SearchUsers(string query, int limit = 20)
        {
            try
            {
                if (string.IsNullOrEmpty(query))
                {
                    return [];
                }

                if (limit < 0) limit = 20;

                var users = await _userRepository.SearchUsers(query, limit);

                List<UserDto> response = users.Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                    loyaltyAccount = u.LoyaltyAccount
                }).ToList();

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
