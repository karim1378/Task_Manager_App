using System.ComponentModel.DataAnnotations;

namespace TaskManagerApp.Application.DTOs
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }



        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
    }
}
