using Microsoft.AspNetCore.Identity;
using TaskManagerApp.Application.Interfaces;
using TaskManagerApp.Domain.Entities;
using TaskManagerApp.Domain.Interfaces;
using TaskManagerApp.Infrastructure.Persistence;
using TaskManagerApp.Infrastructure.Repositories;

namespace TaskManagerApp.Infrastructure.Services
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private ITaskRepository _taskRepository;
        private ITaskOperationRequestRepository _taskOperationRequestRepository;
        private IProjectRepository _projectRepository;

        public UnitOfWork(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public ITaskRepository TaskRepository => _taskRepository ??= new TaskRepository(_context, _userManager);
        public ITaskOperationRequestRepository TaskOperationRequestRepository => _taskOperationRequestRepository ??= new TaskOperationRequestRepository(_context);
        public IProjectRepository ProjectRepository => _projectRepository ??= new ProjectRepository(_context, _userManager);



        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}


