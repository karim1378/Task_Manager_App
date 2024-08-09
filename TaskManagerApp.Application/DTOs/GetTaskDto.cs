using System.ComponentModel.DataAnnotations;
using TaskStatus = TaskManagerApp.Domain.Enums.TaskStatus;
namespace TaskManagerApp.Application.DTOs
{
    public class GetTaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime? Deadline { get; set; }
        public string? AssignRequestDescription { get; set; }
        public string? UnassignRequestDescription { get; set; }
        public string? CompletionRequestDescription { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string CreatorUserName { get; set; }
        public string? AssigneeUserName { get; set; } 
    }
}
