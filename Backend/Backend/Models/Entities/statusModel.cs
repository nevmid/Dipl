using System.Text.Json.Serialization;

namespace Backend.Models.Entities
{
    public class Status
    {
        public int Id {  get; set; }
        public string Name { get; set; } = string.Empty;
        [JsonIgnore]
        public List<Booking> Bookings { get; set; } = [];
        [JsonIgnore]
        public List<Payment> Payments { get; set; } = [];
        [JsonIgnore]
        public List<Ticket> Tickets { get; set; } = [];
    }
}
