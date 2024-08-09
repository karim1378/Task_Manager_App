using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerApp.Application.DTOs;
using TaskManagerApp.Domain.Entities;
using Task = TaskManagerApp.Domain.Entities.Task;


namespace TaskManagerApp.Domain.Interfaces { 

public interface ITaskRepository
    {
        Task<Task?> GetByIdAsync(int id);
        Task<Task?> GetByNameAsync(string name);
        System.Threading.Tasks.Task DeleteAsync(int id);
        System.Threading.Tasks.Task CreateAsync(Task task);
        System.Threading.Tasks.Task UpdateAsync(int id, Task task);
        Task<IEnumerable<Task>> GetUserTasksAsync(int projectId, string userName);
        Task<IEnumerable<Task>> GetProjectTasksAsync(int projectId);
    }

}