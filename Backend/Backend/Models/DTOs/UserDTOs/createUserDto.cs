using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs.UserDTOs
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        [MaxLength(255, ErrorMessage = "Слишком длинный email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        [MinLength(8, ErrorMessage = "Пароль должен состоять минимум из 8 символов")]
        [MaxLength(100, ErrorMessage = "Слишком длинный пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Подтверждение пароля обязательно")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(user|admin)$",
            ErrorMessage = "Доступные роли: 'user', 'admin'")]
        public string Role { get; set; } = "user";
    }
}
