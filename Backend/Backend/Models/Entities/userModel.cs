using System.Text.Json.Serialization;

namespace Backend.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        public List<Booking> Bookings { get; set; } = [];
        [JsonIgnore]
        public LoyaltyAccount LoyaltyAccount { get; set; } = null!;
        public Role Role { get; set; } = null!;
    }
}
