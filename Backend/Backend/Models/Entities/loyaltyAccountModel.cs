using System.Text.Json.Serialization;

namespace Backend.Models.Entities
{
    public class LoyaltyAccount
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public double Balance { get; set; }
        [JsonIgnore]
        public User User { get; set; } = null!;
    }
}
