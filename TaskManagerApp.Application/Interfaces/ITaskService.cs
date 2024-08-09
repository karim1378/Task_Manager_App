using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerApp.Application.DTOs;
using TaskManagerApp.Domain.Entities;
using TaskManagerApp.TaskManagerApp.Application.DTOs;
using Task = System.Threading.Tasks.Task;

namespace TaskManagerApp.Application.Interfaces
{
    public interface ITaskService
    {
        Task<GetTaskDto?> GetTaskByIdAsync(int id);
        Task<GetTaskDto?> GetTaskByNameAsync(string name);
        Task DeleteTaskAsync(int id);
        Task<GetTaskDto> CreateTaskAsync(CreateTaskDto createTaskDto);
        Task<GetTaskDto> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto);
        Task<IEnumerable<GetTaskDto>> GetUserTasksAsync(int projectId, string userName);
        Task<IEnumerable<GetTaskDto>> GetProjectTasksAsync(int projectId);
        Task<TaskOperationRequestDto> AddRequestAsync(TaskOperationRequestDto taskOperationRequestDto);
        Task DeleteRequestAsync(int taskId);
        Task<TaskOperationRequestDto> UpdateRequestAsync(int taskId, TaskOperationRequestDto taskOperationRequestDto);
        Task<IEnumerable<TaskOperationRequestDto>> GetRequestsByProjectIdAsync(int projectId);
        Task<IEnumerable<TaskOperationRequestDto>> GetRequestsByTaskIdAsync(int taskId);
        Task<GetTaskDto> AssignTaskAsync(TaskOperationRequesterDto taskOperationRequesterDto);
        Task<GetTaskDto> UnassignTaskAsync(TaskOperationRequesterDto taskOperationRequesterDto);
        Task<GetTaskDto> CompleteTaskAsync(TaskOperationRequesterDto taskOperationRequesterDto);
    }
}
