using System.ComponentModel.DataAnnotations;
using TaskManagerApp.Domain.Enums;

namespace TaskManagerApp.Application.DTOs
{
    public class TaskOperationRequestDto
    {
        public RequestType Type { get; set; }
        public string Description { get; set; }
        public int TaskId { get; set; }
        public int ProjectId { get; set; }
    }
}
