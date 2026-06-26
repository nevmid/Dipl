using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs.BookingDTOs
{
    public class CreateBookingDto
    {
        [Required]
        public int SessionId { get; set; }

        [Required]
        [MinLength(1)]
        public List<int> SeatIds { get; set; } = new();
        public bool isBonusUsed { get; set; }

    }
}
