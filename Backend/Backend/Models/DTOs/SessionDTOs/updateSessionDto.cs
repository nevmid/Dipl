using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs.SessionDTOs
{
    public class UpdateSessionDto
    {
        [Required(ErrorMessage = "ID фильма обязателен")]
        [Range(1, int.MaxValue, ErrorMessage = "Некорректный ID фильма")]
        public int MovieId { get; set; }
        [Required(ErrorMessage = "ID зала обязателен")]
        [Range(1, int.MaxValue, ErrorMessage = "Некорректный ID зала")]
        public int HallId { get; set; }
        [Required(ErrorMessage = "Время начала обязательно")]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTime? EndTime { get; set; }
        [Required(ErrorMessage = "Цена обязательна")]
        [Range(0.01, 10000, ErrorMessage = "Цена должна быть от 0.01 до 10000")]
        [DataType(DataType.Currency)]
        public double Price { get; set; }
    }
}
