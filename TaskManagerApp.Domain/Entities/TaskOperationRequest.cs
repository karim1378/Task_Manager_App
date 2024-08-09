using TaskManagerApp.Domain.Enums;

namespace TaskManagerApp.Domain.Entities
{
    public class TaskOperationRequest
    {
        public int Id { get; set; }
        public RequestType Type { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int TaskId { get; set; }
        public Task Task { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; } 
    }
}
