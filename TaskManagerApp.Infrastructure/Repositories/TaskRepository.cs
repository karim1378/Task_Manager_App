using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TaskManagerApp.Domain.Interfaces;
using TaskManagerApp.Application.Exceptions;
using Task = TaskManagerApp.Domain.Entities.Task;
using TaskManagerApp.Application.DTOs;
using Microsoft.AspNetCore.Identity;
using TaskManagerApp.Domain.Entities;
using TaskManagerApp.Infrastructure.Persistence;
using System.Xml.Linq;

namespace TaskManagerApp.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public TaskRepository(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async System.Threading.Tasks.Task CreateAsync(Task task)
        {
            try
            {
              await _context.Tasks.AddAsync(task);
              //await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("error in creating new task", ex);
            }
            
        }

        public async System.Threading.Tasks.Task DeleteAsync(int id)
        {
            try
            {
                var task = await _context.Tasks.SingleOrDefaultAsync(t => t.Id == id);
                task.IsDeleted = true;
                _context.Tasks.Update(task);
               //await _context.SaveChangesAsync();
            }catch (Exception ex)
            {
                throw new RepositoryException("error in logical deleting of task", ex);
            }
        }

        public async Task<Task?> GetByIdAsync(int id)
        {
            try
            {
                var task = await _context.Tasks
                    .Include(t => t.Assignee)
                    .Include(t => t.Project)
                    .Include(t => t.Creator)
                    .SingleOrDefaultAsync(t => t.Id == id && t.IsDeleted == false);
                return task;
            }catch (Exception ex)
            {
                throw new RepositoryException("error in task retirving",ex);
            }
        }

        public async Task<Task?> GetByNameAsync(string name)
        {
            try
            {
                var task = await _context.Tasks
                    .Include(t => t.Assignee)
                    .Include(t => t.Project)
                    .Include(t => t.Creator)
                    .SingleOrDefaultAsync(t => t.Title == name && t.IsDeleted == false);
                return task;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("error in task retirving",ex);
            }
        }

        public async Task<IEnumerable<Task>> GetProjectTasksAsync(int projectId)
        {
            try
            {
                var tasks = await _context.Tasks
                    .Include(t => t.Assignee)
                    .Include(t => t.Project)
                    .Include(t => t.Creator)
                    .Where(t => t.ProjectId == projectId && t.IsDeleted == false)
                    .ToListAsync();
                return tasks;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("error in task retirving", ex);
            }
        }

        public async Task<IEnumerable<Task>> GetUserTasksAsync(int projectId, string userName)
        {
             try
            {
                var tasks = await _context.Tasks
                    .Include(t => t.Assignee)
                    .Include(t => t.Project)
                    .Include(t => t.Creator)
                    .Where(t => t.ProjectId == projectId && t.Assignee.UserName == userName && t.IsDeleted == false)
                    .ToListAsync();
                return tasks;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("error in task retriving", ex);
            };
        }

        public async System.Threading.Tasks.Task UpdateAsync(int id, Task task)
        {
            try
            {
                _context.Tasks.Update(task);
                await _context.SaveChangesAsync();
               
            }
            catch (Exception ex)
            {
                throw new RepositoryException("task dont update",ex);
            }
        }
    }
}
