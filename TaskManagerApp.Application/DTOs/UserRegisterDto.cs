using System.ComponentModel.DataAnnotations;

namespace TaskManagerApp.Application.DTOs
{
    public class UserRegisterDto
    {
        
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }



        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }



        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters")]
        public string UserName { get; set; }



        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }



        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
    }
}
