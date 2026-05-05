using Backend.Models.Entities;
using System.Text.Json.Serialization;

namespace Backend.Models.DTOs.SessionDTOs
{
    public class sessionsResponseDto
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int HallId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double Price { get; set; }
        public Movie? Movie { get; set; }
        public Hall? Hall { get; set; }
        public List<Booking>? Bookings { get; set; }
    }
}
