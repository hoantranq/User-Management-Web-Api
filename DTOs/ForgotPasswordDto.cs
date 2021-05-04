using System.ComponentModel.DataAnnotations;

namespace UserManagement_Backend.DTOs
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
