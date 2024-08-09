using System.ComponentModel.DataAnnotations;

namespace TaskManagerApp.Application.DTOs
{
    public class UpdateProjectDto
    {
        public string? Name { get; set; }

        public string? Description { get; set; }


        [DataType(DataType.Date, ErrorMessage = "date format is wrong.")]
        public DateTime? DeadLine { get; set; }
    }
}
