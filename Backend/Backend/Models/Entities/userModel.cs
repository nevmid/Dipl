using System.Text.Json.Serialization;

namespace Backend.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "user";
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [JsonIgnore]
        public List<Booking> Bookings { get; set; } = [];
        [JsonIgnore]
        public LoyaltyAccount LoyaltyAccount { get; set; } = null!;
    }
}
