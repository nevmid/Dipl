using System.Text.Json.Serialization;

namespace Backend.Models.Entities
{
    public class Hall
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<Session> Sessions { get; set; } = [];
        public List<Seat> Seats { get; set; } = [];
    }
}
