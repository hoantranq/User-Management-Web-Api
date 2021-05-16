using System.ComponentModel.DataAnnotations;

namespace UserManagement_Backend.DTOs
{
    public class UserForResetPasswordDto
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "New Password and Comfirm Password does not match!")]
        public string ConfirmPassword { get; set; }
    }
}
