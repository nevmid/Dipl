using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Backend.Models.DTOs.HallDTOs
{
    public class RequestHallDto
    {
        [Required(ErrorMessage = "Название зала обязательно")]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required(ErrorMessage="Количество рядов для мест обязательно")]
        [Range(0, 100, ErrorMessage = "Количество рядов от 0 до 100")]
        public int RowNum { get; set; }
        [Required(ErrorMessage = "Количество мест в ряду обязательно")]
        [Range(0, 100, ErrorMessage = "Количество мест от 0 до 100")]
        public int ColNum { get; set; }
    }
}
