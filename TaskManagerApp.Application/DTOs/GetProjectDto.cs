using System.ComponentModel.DataAnnotations;

namespace TaskManagerApp.Application.DTOs
{
    public class GetProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? Deadline { get; set; }
        public string OwnerUserName { get; set; }
    }
}
