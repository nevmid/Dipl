using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Backend.Interfaces;
using Backend.Models.DTOs.UserDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetMe()
        {
            try
            {
                var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var user = await _userService.GetProfileAsync(id);

                if (user == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Профиль не найден"
                    });
                }

                return Ok(new { success = true, user });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Server Error | User Profile"
                });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 25)
        {
            try
            {
                var (users, totalCount) = await _userService.GetUsersAsync(page, pageSize);

                var hasMore = (page * pageSize) < totalCount;

                return Ok(new
                {
                    users,
                    pagination = new
                    {
                        currentPage = page,
                        hasMore,
                        pageSize,
                        totalCount,
                        nextPage = hasMore ? (page + 1) : (int?)null
                    }
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Server Error | Get Users" });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("admin/users")]
        public async Task<IActionResult> SearchUsers(
            [FromQuery] string search,
            [FromQuery] int limit = 10)
        {
            try
            {
                var users = await _userService.SearchUsers(search, limit);

                return Ok(users);
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Server Error | Search Users" });
            }
        }

        [Authorize]
        [HttpGet("bookings")]
        public async Task<IActionResult> GetMyBookings()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var bookings = await _userService.GetUserBookingsAsync(userId);

                return Ok(bookings);

            }
            catch (Exception)
            {
                return StatusCode(500, "Server Error | Get User's Bookings");
            }
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteAccount()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var deletedUser = await _userService.DeleteUser(userId);

                if (deletedUser == null)
                {
                    return NotFound();
                }

                return Ok(new
                {
                    message = "Аккаунт удалён",
                    deletedUser
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Server Error | Delete Account" });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}/role")]
        public async Task<IActionResult> ChangeRole(int id, [FromBody] ChangeRoleDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var (success, email) = await _userService.ChangeRole(id, request);

                if (!success)
                {
                    return NotFound(new {success = false, message = "Пользователь не найден"});
                }

                return Ok(new {message = "Роль изменена", email});
            }
            catch(ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Server Error | Change Role" });
            }
        }

        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var result = await _userService.ChangePassword(userId, request);

                if (!result)
                {
                    return NotFound(new { message = "Пользователь не найден" });
                }

                return Ok(new { message = "Пароль изменён" });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Server Error | Change Password" });
            }
        }

        [Authorize]
        [HttpPut("change-email")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var result = await _userService.ChangeEmail(userId, request);

                if (!result)
                {
                    return NotFound(new { message = "Пользователь не найден" });
                }

                return Ok(new { message = "Email изменён" });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new {success = false, message = "Server Error | Change Email" });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                await _userService.InitiatePasswordReset(request);
                return Ok(new { message = "Ссылка для смены пароля отправлена на почту" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Server Error | Send Reset Link" });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var result = await _userService.ResetPassword(request);

                if (!result)
                    return BadRequest(new { message = "Ошибка при сбросе пароля" });

                return Ok(new { message = "Пароль восстановлен" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Server Error | Reset Password" });
            }
        }
    }
}
