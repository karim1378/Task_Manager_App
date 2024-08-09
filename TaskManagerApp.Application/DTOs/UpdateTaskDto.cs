 using System.ComponentModel.DataAnnotations;

namespace TaskManagerApp.TaskManagerApp.Application.DTOs
{
    public class UpdateTaskDto
    {
        public string? Title { get; set; }


        public string? Description { get; set; }


        [Range(1, 100, ErrorMessage = "Value must be between 1 and 100.")]
        public int Priority { get; set; }


        [DataType(DataType.Date, ErrorMessage = "date format is wrong.")]
        public DateTime? Deadline { get; set; }

        public int ProjectId { get; set; }

    }
}
