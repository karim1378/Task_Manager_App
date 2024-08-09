using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagerApp.Application.Exceptions;
using TaskManagerApp.Domain.Entities;
using TaskManagerApp.Domain.Enums;
using TaskManagerApp.Domain.Interfaces;
using TaskManagerApp.Infrastructure.Persistence;
using Task = System.Threading.Tasks.Task;

namespace TaskManagerApp.Infrastructure.Repositories
{
    public class TaskOperationRequestRepository : ITaskOperationRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskOperationRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddRequestAsync(TaskOperationRequest taskOperationRequest)
        {
            try
            {
                await _context.TaskOperationRequests.AddAsync(taskOperationRequest);
               // await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("error in add request",ex);
            }
        }

        public async Task DeleteRequestAsync(UserRole role, string? userName, int taskId)
        {
            try
            {
                if (role == UserRole.Owner)
                {

                    var requests = await _context.TaskOperationRequests
                        .Where(r => r.TaskId == taskId)
                        .ToListAsync();

                    if (requests.Count == 0)
                    {
                        throw new ServiceException("No requests found for the specified task ID.");
                    }

                    _context.TaskOperationRequests.RemoveRange(requests);
                }
                else
                {
                    if (userName == null)
                    {
                        throw new ServiceException("User name must be provided for non-Owner roles.");
                    }

                    var request = await _context.TaskOperationRequests
                        .SingleOrDefaultAsync(r => r.TaskId == taskId && r.User.UserName == userName);

                    if (request == null)
                    {
                        throw new ServiceException("Request not found for the specified task ID and user name.");
                    }

                    _context.TaskOperationRequests.Remove(request);
                }
            }
            catch (Exception ex)
            {
                throw new RepositoryException("error in delete request", ex);
            }
        }


        public async Task<IEnumerable<TaskOperationRequest>> GetRequestsByProjectIdAsync(UserRole role, string? userName, int projctId)
        {
            try
            {
                if (role == UserRole.Owner)
                {
                    return await _context.TaskOperationRequests.Where(tor => tor.ProjectId == projctId).ToListAsync();
                }
                else
                {
                    if (userName == null)
                    {
                        throw new ServiceException("User name must be provided for non-Owner roles.");
                    }
                    return await _context.TaskOperationRequests
                        .Where(tor => tor.User.UserName == userName && tor.ProjectId == projctId)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new RepositoryException("error in get project all requests",ex);
            }
        }

        public async Task<IEnumerable<TaskOperationRequest>> GetRequestsByTaskIdAsync(UserRole role, string? userName, int taskId)
        {
            try
            {
                if (role == UserRole.Owner)
                {
                    return await _context.TaskOperationRequests
                        .Where(r => r.TaskId == taskId)
                        .ToListAsync();
                }
                else
                {
                    if (userName == null)
                    {
                        throw new ServiceException("User name must be provided for non-Owner roles.");
                    }
                    return await _context.TaskOperationRequests
                        .Where(r => r.TaskId == taskId && r.User.UserName == userName)
                        .ToListAsync();
                }
            }catch (Exception ex)
            {
                throw new RepositoryException("error in get requests by task id", ex);
            }
        }


        public async Task UpdateRequestAsync(TaskOperationRequest taskOperationRequest)
        {
            try
            {
                _context.TaskOperationRequests.Update(taskOperationRequest);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("error in update request",ex);
            }
        }
    }
}
