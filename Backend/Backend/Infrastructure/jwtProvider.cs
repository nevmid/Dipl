using Backend.Models.DTOs.UserDTOs;
using Backend.Models.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend.Infrastructure
{
    public class JwtProvider
    {
        private readonly JwtOptions _jwtSettings;
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public class TicketJwtData
        {
            public int BookingId { get; set; }
            public string TicketNumber { get; set; } = string.Empty;
            public string MovieTitle { get; set; } = string.Empty;
            public int DurationMinutes { get; set; }
            public DateTime SessionTime { get; set; }
            public string HallName { get; set; } = string.Empty;
            public string UserEmail { get; set; } = string.Empty;
        }

        public JwtProvider(IOptions<JwtOptions> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
            _tokenHandler = new JwtSecurityTokenHandler();
        }
        public string generateAuthToken(UserDto user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("type", "auth")
            };

            var expiresAt = DateTime.UtcNow.AddHours(_jwtSettings.ExpiresHours);

            return GenerateToken(claims, expiresAt);
        }

        public string generateResetPasswordToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("type", "reset_password")
            };

            var expiresAt = DateTime.UtcNow.AddHours(1);

            return GenerateToken(claims, expiresAt);
        }

        public string generateTicketToken(TicketJwtData data)
        {
            var claims = new List<Claim>
            {
                new Claim("type", "ticket"),
                new Claim("booking_id", data.BookingId.ToString()),
                new Claim("ticket_number", data.TicketNumber),
                new Claim("movie_title", data.MovieTitle),
                new Claim("session_time", data.SessionTime.ToString()),
                new Claim("hall_name", data.HallName),
                new Claim("user_email", data.UserEmail),
                new Claim("jti", Guid.NewGuid().ToString())
            };

            var expiresAt = data.SessionTime.AddMinutes(data.DurationMinutes + 30);

            return GenerateToken(claims, expiresAt);
        }

        private string GenerateToken(List<Claim> claims,  DateTime expiresAt)
        {
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: expiresAt,
                signingCredentials: signingCredentials
                );

            return _tokenHandler.WriteToken(token);
        }

        public (bool isValid, int userId, string email, string type) ValidateToken(string token)
        {
            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateActor = false,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
                };

                var principal = _tokenHandler.ValidateToken(
                    token,
                    validationParameters,
                    out SecurityToken validatedToken);

                var userId = int.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var email = principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
                var type = principal.FindFirstValue("type") ?? string.Empty;

                return (true, userId, email, type);
            }
            catch (Exception)
            {
                return (false, 0, string.Empty, string.Empty);
            }
        }

        public (bool isValid, TicketJwtData? data, string error) ValidateTicketToken(string token)
        {
            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateActor = false,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
                };

                var principal = _tokenHandler.ValidateToken(
                    token,
                    validationParameters,
                    out SecurityToken validatedToken);

                var type = principal.FindFirst("type")?.Value;
                if (type != "ticket")
                    return (false, null, "Невалидный тип токена");

                var data = new TicketJwtData
                {
                    BookingId = int.Parse(principal.FindFirst("booking_id")?.Value ?? "0"),
                    TicketNumber = principal.FindFirst("ticket_number")?.Value ?? "",
                    MovieTitle = principal.FindFirst("movie_title")?.Value ?? "",
                    SessionTime = DateTime.Parse(principal.FindFirst("session_time")?.Value ?? DateTime.UtcNow.ToString()),
                    HallName = principal.FindFirst("hall_name")?.Value ?? "",
                    UserEmail = principal.FindFirst("user_email")?.Value ?? ""
                };

                return (true, data, string.Empty);
            }
            catch (SecurityTokenExpiredException)
            {
                return (false, null, "Билет недействителен");
            }
            catch (Exception ex)
            {
                return (false, null, $"Невалидный билет: {ex.Message}");
            }
        }
    }
}
