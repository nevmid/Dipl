using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs.UserDTOs
{
    public class ChangeRoleDto
    {
        [Required]
        [RegularExpression(@"^(user|admin)$",
            ErrorMessage = "Доступные роли: 'user', 'admin'")]
        public string Role { get; set; } = "user";
    }
}
