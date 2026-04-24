using System.ComponentModel.DataAnnotations;

namespace DB.Entity
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Current Password is required")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New Password is required")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Retype New Password is required")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
