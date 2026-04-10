using System.Text.Json.Serialization;

namespace Backend.Models.Entities
{
    public class Seat
    {
        public int Id { get; set; }
        public int HallId { get; set; }
        public int RowNum { get; set; }
        public int ColNum { get; set; }
        public string Type { get; set; } = "standard";
        [JsonIgnore]
        public Hall Hall { get; set; } = null!;
        [JsonIgnore]
        public List<BookingSeat> BookingSeats { get; set; } = [];
    }
}
