using System.ComponentModel.DataAnnotations;

namespace TaskManagerApp.Application.DTOs
{
    public class CreateProjectDto
    {
        [Required(ErrorMessage ="project name is required")]
        public string Name { get; set; }



        [Required(ErrorMessage = "project Description is required")]
        public string Description { get; set; }




        [Required(ErrorMessage = "project deadline is required")]
        [DataType(DataType.Date, ErrorMessage = "date format is wrong.")]
        public DateTime DeadLine { get; set; }
    }
}
