namespace TaskManagerApp.Application.DTOs
{
    public class TaskOperationRequesterDto
    {
        public int TaskId { get; set; }
        public int ProjectId { get; set; }
        public string UserName { get; set; }
    }
}
