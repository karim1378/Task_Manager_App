using System.ComponentModel.DataAnnotations;

namespace TaskManagerApp.Application.DTOs
{
    public class CreateTaskDto
    {
        [Required(ErrorMessage = "task title is required")]
        public string Title { get; set; }


        [Required(ErrorMessage = "task description is required")]
        public string Description { get; set; }


        [Required(ErrorMessage = "task priority is required")]
        [Range(1, 100, ErrorMessage = "Value must be between 1 and 100.")]
        public int Priority { get; set; }


        [Required(ErrorMessage = "task deadline date is required")]
        [DataType(DataType.Date, ErrorMessage = "date format is wrong.")]
        public DateTime Deadline { get; set; }


        [Required(ErrorMessage = "project id is required")]
        public int ProjectId { get; set; }
    }
}
