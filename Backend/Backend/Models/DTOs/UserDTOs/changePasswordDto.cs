using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs.UserDTOs
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Текущий пароль обязателен")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Новый пароль обязателен")]
        [MinLength(8, ErrorMessage = "Новый пароль должен состоять минимум из 8 символов")]
        [MaxLength(100, ErrorMessage = "Слишком длинный новый пароль")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;
    }
}
