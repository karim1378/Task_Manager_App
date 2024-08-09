using System.ComponentModel.DataAnnotations;

namespace TaskManagerApp.Application.DTOs
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }



        [Required(ErrorMessage = "token is required")]
        public string Token { get; set; }



        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        [RegularExpression(@"^(?=.*[0-9])(?=.*[^a-zA-Z]).{6,100}$", ErrorMessage = "Password must be at least 6 characters long and contain at least one digit and one non-alphabetic character")]
        public string NewPassword { get; set; }
    }
}
