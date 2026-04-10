using Backend.Models.Entities;

namespace Backend.Models.DTOs.UserDTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Role { get; set; } = "user";
        public LoyaltyAccount? loyaltyAccount { get; set; }
    }
}
