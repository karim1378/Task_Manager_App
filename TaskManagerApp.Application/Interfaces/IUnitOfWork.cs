using TaskManagerApp.Domain.Interfaces;
using TaskManagerApp.Infrastructure.Repositories;

namespace TaskManagerApp.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ITaskRepository TaskRepository { get; }
        ITaskOperationRequestRepository TaskOperationRequestRepository { get; }
        IProjectRepository ProjectRepository { get; }

        Task<int> SaveChangesAsync();
    }

}
