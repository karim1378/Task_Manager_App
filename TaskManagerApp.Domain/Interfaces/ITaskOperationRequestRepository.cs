using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerApp.Application.DTOs;
using TaskManagerApp.Domain.Entities;
using TaskManagerApp.Domain.Enums;
using Task = System.Threading.Tasks.Task;


namespace TaskManagerApp.Domain.Interfaces
{
    public interface ITaskOperationRequestRepository
    {
        Task<IEnumerable<TaskOperationRequest>> GetRequestsByTaskIdAsync(UserRole role, string? userName, int taskId);
        Task<IEnumerable<TaskOperationRequest>> GetRequestsByProjectIdAsync(UserRole role, string? userName, int projectId);
        Task AddRequestAsync(TaskOperationRequest taskOperationRequest);
        Task UpdateRequestAsync(TaskOperationRequest taskOperationRequest);
        Task DeleteRequestAsync(UserRole role, string? userName, int taskId);


    }
}
