using System.ComponentModel.DataAnnotations;

namespace TaskManagerApp.Application.DTOs
{
    public class UpdateProfileDto
    {

        public string? FirstName { get; set; }



        public string? LastName { get; set; }



        [StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters")]
        public string? UserName { get; set; }



        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        public string? Password { get; set; }



        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }
    }
}
