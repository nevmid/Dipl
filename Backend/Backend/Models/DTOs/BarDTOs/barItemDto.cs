using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs.BarDTOs
{
    public class barItemDto
    {
        [Required(ErrorMessage = "ID категории обязательно")]
        [Range(1, int.MaxValue, ErrorMessage = "Некорректный ID категории")]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Название товара обязательно")]
        [MaxLength(100, ErrorMessage ="Слишком длинное название")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage ="Цена обязательна")]
        [Range(0.01, 10000, ErrorMessage = "Цена должна быть от 0.01 до 10000")]
        [DataType(DataType.Currency)]
        public double Price { get; set; }
        
        public bool IsAvailable { get; set; } = true;
    }
}
