using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs.UserDTOs
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public string Email { get; set; } = string.Empty;
    }
}
