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

        public JwtProvider(IOptions<JwtOptions> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
            _tokenHandler = new JwtSecurityTokenHandler();
        }
        public string generateAuthToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("type", "auth")
            };

            int expiresHours = _jwtSettings.ExpiresHours;

            return GenerateToken(claims, expiresHours);
        }

        public string generateResetPasswordToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("type", "reset_password")
            };

            int expiresHours = 1;

            return GenerateToken(claims, expiresHours);
        }

        private string GenerateToken(List<Claim> claims, int expiresHours)
        {
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expiresHours),
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
    }
}
