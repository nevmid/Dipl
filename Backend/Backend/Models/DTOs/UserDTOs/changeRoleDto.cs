using System.ComponentModel.DataAnnotations;

namespace Backend.Models.DTOs.UserDTOs
{
    public class ChangeRoleDto
    {
        [Required]
        [RegularExpression(@"^(user|admin|staff)$",
            ErrorMessage = "Доступные роли: 'user', 'admin', 'staff'")]
        public string Role { get; set; } = "user";
    }
}
