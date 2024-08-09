using TaskManagerApp.Domain.Enums;
namespace TaskManagerApp.Domain.Entities
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public Enums.TaskStatus Status { get; set; }
        public DateTime? Deadline { get; set; }
        public bool IsDeleted { get; set; }
        public string? AssignRequestDescription { get; set; }
        public string? UnassignRequestDescription { get; set; }
        public string? CompletionRequestDescription { get; set; }
        public string CreatorId { get; set; }
        public User Creator { get; set; }
        public string? AssigneeId { get; set; }
        public User? Assignee { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public ICollection<TaskOperationRequest>? TaskOperationRequests { get; set; }

    }
}

