using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerApp.Application.DTOs;
using TaskManagerApp.Application.Interfaces;
using TaskManagerApp.Domain.Entities;
using TaskManagerApp.Domain.Enums;
using TaskManagerApp.Infrastructure.Repositories;
using TaskManagerApp.Application.Exceptions;
using Task = System.Threading.Tasks.Task;
using TaskManagerApp.TaskManagerApp.Application.DTOs;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace TaskManagerApp.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TaskService(IUnitOfWork unitOfWork, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<TaskOperationRequestDto> AddRequestAsync(TaskOperationRequestDto taskOperationRequestDto)
        {
            try
            {
                var user = await CurrentRequester();

                if (user == null)
                {
                    throw new ServiceException("user not found, please login");
                }

                var isProjectMember = await _unitOfWork.ProjectRepository.IsProjectMemberAsync(taskOperationRequestDto.ProjectId, user.Id);
                var requests = await _unitOfWork.TaskOperationRequestRepository.GetRequestsByTaskIdAsync(UserRole.Member, user.UserName, taskOperationRequestDto.TaskId);
                var previousRequest = requests.SingleOrDefault(tor => tor.UserId == user.Id);
                var task = await _unitOfWork.TaskRepository.GetByIdAsync(taskOperationRequestDto.TaskId);
                if (task == null)
                {
                    throw new ServiceException("task not found");
                }

                var request = new TaskOperationRequest()
                {
                    Type = taskOperationRequestDto.Type,
                    Description = taskOperationRequestDto.Description,
                    UserId = user.Id,
                    ProjectId = taskOperationRequestDto.ProjectId,
                    TaskId = taskOperationRequestDto.TaskId,
                };

                if (taskOperationRequestDto.Type == RequestType.Assign
                    && isProjectMember
                    && previousRequest == null
                    && task.AssignRequestDescription == null
                    && task.UnassignRequestDescription == null
                    && task.CompletionRequestDescription == null)
                {
                    await _unitOfWork.TaskOperationRequestRepository.AddRequestAsync(request);
                    await _unitOfWork.SaveChangesAsync();
                    return taskOperationRequestDto;

                }
                else if ((taskOperationRequestDto.Type == RequestType.Unassign || taskOperationRequestDto.Type == RequestType.Completion)
                    && task.AssigneeId == user.Id
                    && previousRequest == null
                    && task.AssignRequestDescription != null
                    && task.UnassignRequestDescription == null
                    && task.CompletionRequestDescription == null)
                {
                    await _unitOfWork.TaskOperationRequestRepository.AddRequestAsync(request);
                    await _unitOfWork.SaveChangesAsync();
                    return taskOperationRequestDto;
                }

                else
                {
                    throw new ServiceException("you aren't this project Member or you aren't this task assignee or you apply false request to this task");
                }
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("error in add request", ex);
            }
        }
            
        

        public async Task<GetTaskDto> AssignTaskAsync(TaskOperationRequesterDto taskOperationRequesterDto)
        {
            try
            {
                var isProjectOwner = await IsProjectOwner(taskOperationRequesterDto.ProjectId);
                if (isProjectOwner)
                {
                    var task = await _unitOfWork.TaskRepository.GetByIdAsync(taskOperationRequesterDto.TaskId);
                    if (task == null)
                    {
                        throw new ServiceException("task not found");
                    }
                    if(task.AssigneeId == null)
                    {
                        var request = _unitOfWork.TaskOperationRequestRepository.GetRequestsByTaskIdAsync(UserRole.Member, taskOperationRequesterDto.UserName, taskOperationRequesterDto.TaskId).Result.FirstOrDefault();
                        if (request != null && request.Type == RequestType.Assign)
                        {
                            
                            task.AssigneeId = request.UserId;
                            task.Status = Domain.Enums.TaskStatus.InProgress;
                            task.AssignRequestDescription = request.Description;
                            await _unitOfWork.TaskRepository.UpdateAsync(task.Id, task);
                            await _unitOfWork.SaveChangesAsync();
                            

                            await _unitOfWork.TaskOperationRequestRepository.DeleteRequestAsync(UserRole.Owner,null, task.Id);
                            await _unitOfWork.SaveChangesAsync();
                            return new GetTaskDto
                            {
                                Id = task.Id,
                                Title = task.Title,
                                Description = task.Description,
                                Priority = task.Priority,
                                Status = task.Status,
                                Deadline = task.Deadline,
                                AssignRequestDescription = task.AssignRequestDescription,
                                UnassignRequestDescription = task.UnassignRequestDescription,
                                CompletionRequestDescription = task.CompletionRequestDescription,
                                ProjectId = task.ProjectId,
                                ProjectName = task.Project.Name,
                                CreatorUserName = task.Creator.UserName,
                                AssigneeUserName = taskOperationRequesterDto.UserName

                            };
                        }
                        else
                        {
                            throw new ServiceException("user assign request not found");
                        }
                    }
                    else
                    {
                        throw new ServiceException("previously task assigned to another member");
                    }
                }
                else { throw new ServiceException("you don't have access permission to this section"); }
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("error in task assign", ex);
            }
        }

        public async Task<GetTaskDto> CompleteTaskAsync(TaskOperationRequesterDto taskOperationRequesterDto)
        {
            try
            {
                var isProjectOwner = await IsProjectOwner(taskOperationRequesterDto.ProjectId);
                if (isProjectOwner)
                {
                    var task = await _unitOfWork.TaskRepository.GetByIdAsync(taskOperationRequesterDto.TaskId);
                    if (task == null)
                    {
                        throw new ServiceException("task not found");
                    }
                    if (task.Assignee?.UserName == taskOperationRequesterDto.UserName)
                    {
                        var request = _unitOfWork.TaskOperationRequestRepository.GetRequestsByTaskIdAsync(UserRole.Member, taskOperationRequesterDto.UserName, taskOperationRequesterDto.TaskId)
                                                        .Result.FirstOrDefault();
                        if (request != null && request.Type == RequestType.Completion)
                        {
                            task.Status = Domain.Enums.TaskStatus.Completed;
                            task.CompletionRequestDescription = request.Description;
                            await _unitOfWork.TaskRepository.UpdateAsync(task.Id, task);
                            await _unitOfWork.SaveChangesAsync();

                            await _unitOfWork.TaskOperationRequestRepository.DeleteRequestAsync(UserRole.Owner,null, task.Id);
                            await _unitOfWork.SaveChangesAsync();
                            return new GetTaskDto
                            {
                                Id = task.Id,
                                Title = task.Title,
                                Description = task.Description,
                                Priority = task.Priority,
                                Status = task.Status,
                                Deadline = task.Deadline,
                                AssignRequestDescription = task.AssignRequestDescription,
                                UnassignRequestDescription = task.UnassignRequestDescription,
                                CompletionRequestDescription = task.CompletionRequestDescription,
                                ProjectId = task.ProjectId,
                                ProjectName = task.Project.Name,
                                CreatorUserName = task.Creator.UserName,
                                AssigneeUserName = task.Assignee.UserName

                            };
                        }
                        else
                        {
                            throw new ServiceException("user task completion request not found");
                        }
                    }
                    else
                    {
                        throw new ServiceException("this user can't complete this project");
                    }
                }
                else
                { 
                    throw new ServiceException("you don't have access permission to this section"); 
                }
            }
            catch (RepositoryException ex)
            {
               throw new ServiceException( "error in complete task", ex);
            }
        }

        public async Task<GetTaskDto> CreateTaskAsync(CreateTaskDto createTaskDto)
        {
            try
            {
                var creator = await CurrentRequester();
                var isProjectOwner = await IsProjectOwner(createTaskDto.ProjectId);
                if (isProjectOwner)
                {
                    var task = new Domain.Entities.Task
                    {
                        Title = createTaskDto.Title,
                        Description = createTaskDto.Description,
                        Priority = createTaskDto.Priority,
                        Status = Domain.Enums.TaskStatus.Pending,
                        Deadline = createTaskDto.Deadline,
                        CreatorId = creator.Id,
                        ProjectId = createTaskDto.ProjectId
                    };
                   await _unitOfWork.TaskRepository.CreateAsync(task);
                   await _unitOfWork.SaveChangesAsync();
                    return new GetTaskDto
                    {
                        Id = task.Id,
                        Title = task.Title,
                        Description = task.Description,
                        Priority = task.Priority,
                        Status = task.Status,
                        Deadline = task.Deadline,
                        AssignRequestDescription = null,
                        UnassignRequestDescription = null,
                        CompletionRequestDescription = null,
                        ProjectId = task.ProjectId,
                        ProjectName = _unitOfWork.ProjectRepository.GetByIdAsync(task.ProjectId).Result.Name,
                        CreatorUserName = creator.UserName,
                        AssigneeUserName = null
                    };
                }
                else 
                { throw new ServiceException("you can not create task for this project"); }
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("error in create task",ex);
            }
        }

        public async Task DeleteRequestAsync(int taskId)
        {
            try
            {
                var currentRequester = await CurrentRequester(); 
                var task = await _unitOfWork.TaskRepository.GetByIdAsync(taskId);

                if(task == null)
                {
                    throw new ServiceException("task not found to deleting its request");
                }

                if (await IsProjectOwner(task.ProjectId))
                {
                    await _unitOfWork.TaskOperationRequestRepository.DeleteRequestAsync(UserRole.Owner, null, taskId);
                    await _unitOfWork.SaveChangesAsync();
                }
                else
                {
                    await _unitOfWork.TaskOperationRequestRepository.DeleteRequestAsync(UserRole.Member, currentRequester.UserName, taskId);
                    await _unitOfWork.SaveChangesAsync();
                }


            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("an error occurred in request deleting", ex);
            }
        }

        public async Task DeleteTaskAsync(int taskId)
        {
            try
            {
                var task = await _unitOfWork.TaskRepository.GetByIdAsync(taskId);
                if(task == null)
                {
                    throw new ServiceException("task not found to deleting");
                }
                var isProjectOwner = await IsProjectOwner(task.ProjectId);
                if (isProjectOwner)
                {
                   await _unitOfWork.TaskRepository.DeleteAsync(taskId);
                    await _unitOfWork.SaveChangesAsync();   
                }
                else
                {
                    throw new ServiceException("you can't delete this task");
                }

            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("an error occurred in task deleting", ex);
            }
        }

        public async Task<IEnumerable<TaskOperationRequestDto>> GetRequestsByProjectIdAsync(int projectId)
        {
            try
            {
                var currentRequester = await CurrentRequester();
                if(currentRequester == null)
                {
                    throw new ServiceException("please login");
                }
                else if (await IsProjectOwner(projectId))
                {
                   var requests =  await _unitOfWork.TaskOperationRequestRepository.GetRequestsByProjectIdAsync(UserRole.Owner, null, projectId);
                    return requests.Select(tor => new TaskOperationRequestDto
                    {
                        Type = tor.Type,
                        Description = tor.Description,
                        TaskId = tor.TaskId,
                        ProjectId = tor.ProjectId
                    });
                }
                else {
                    var requests = await _unitOfWork.TaskOperationRequestRepository.GetRequestsByProjectIdAsync(UserRole.Member, currentRequester.UserName, projectId);
                    return requests.Select(tor => new TaskOperationRequestDto
                    {
                        Type = tor.Type,
                        Description = tor.Description,
                        TaskId = tor.TaskId,
                        ProjectId = tor.ProjectId
                    });
                }

            }catch (RepositoryException ex)
            {
                throw new ServiceException("an error occurred in retrieving requests", ex);
            }
        }

        public async Task<IEnumerable<GetTaskDto>> GetProjectTasksAsync(int projectId)
        {
            try
            {
                var tasks = await _unitOfWork.TaskRepository.GetProjectTasksAsync(projectId);
                return tasks.Select(task => new GetTaskDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Priority = task.Priority,
                    Status = task.Status,
                    Deadline = task.Deadline,
                    ProjectId = task.ProjectId,
                    ProjectName = task.Project.Name,
                    CreatorUserName = task.Creator.UserName,
                    AssigneeUserName = task.Assignee?.UserName
                });
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("error in get project tasks", ex);
            }
        }

        public async Task<IEnumerable<TaskOperationRequestDto>> GetRequestsByTaskIdAsync(int taskId)
        {
            try
            {
                var currentRequester = await CurrentRequester();
                var task = await _unitOfWork.TaskRepository.GetByIdAsync(taskId);

                if (task == null)
                {
                    throw new ServiceException("not found any task by this id");
                }

                if (currentRequester == null)
                {
                    throw new ServiceException("please login");
                }
                else if (await IsProjectOwner(task.ProjectId))
                {
                    var requests = await _unitOfWork.TaskOperationRequestRepository.GetRequestsByTaskIdAsync(UserRole.Owner, null, taskId);
                    return requests.Select(tor => new TaskOperationRequestDto
                    {
                        Type = tor.Type,
                        Description = tor.Description,
                        TaskId = tor.TaskId,
                        ProjectId = tor.ProjectId
                    });
                }
                else
                {
                    var requests = await _unitOfWork.TaskOperationRequestRepository.GetRequestsByTaskIdAsync(UserRole.Member, currentRequester.UserName, taskId);
                    return requests.Select(tor => new TaskOperationRequestDto
                    {
                        Type = tor.Type,
                        Description = tor.Description,
                        TaskId = tor.TaskId,
                        ProjectId = tor.ProjectId
                    });
                }

            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("an error occurred in retrieving requests", ex);
            }
        }

        public async Task<GetTaskDto?> GetTaskByIdAsync(int id)
        {
            try { 
                var task = await _unitOfWork.TaskRepository.GetByIdAsync(id);
                if (task == null)
                {
                    return null;
                }
                var getTaskDto = new GetTaskDto();

                getTaskDto.Id = task.Id;
                getTaskDto.Title = task.Title;
                getTaskDto.Description = task.Description;
                getTaskDto.Priority = task.Priority;
                getTaskDto.Status = task.Status;
                getTaskDto.Deadline = task.Deadline;
                getTaskDto.ProjectId = task.ProjectId;
                getTaskDto.ProjectName = task.Project.Name;
                getTaskDto.CreatorUserName = task.Creator.UserName;
                if(task.Assignee != null)
                {
                    getTaskDto.AssigneeUserName = task.Assignee.UserName;
                }
                return getTaskDto;
                
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("error in get task by id", ex);
            }
        }

        public async Task<GetTaskDto?> GetTaskByNameAsync(string name)
        {
            try
            {
                var task = await _unitOfWork.TaskRepository.GetByNameAsync(name);
                if (task == null) {  return null; }
                var getTaskDto = new GetTaskDto();

                getTaskDto.Id = task.Id;
                getTaskDto.Title = task.Title;
                getTaskDto.Description = task.Description;
                getTaskDto.Priority = task.Priority;
                getTaskDto.Status = task.Status;
                getTaskDto.Deadline = task.Deadline;
                getTaskDto.ProjectId = task.ProjectId;
                getTaskDto.ProjectName = task.Project.Name;
                getTaskDto.CreatorUserName = task.Creator.UserName;
                if (task.Assignee != null)
                {
                    getTaskDto.AssigneeUserName = task.Assignee.UserName;
                }
                return getTaskDto;
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("error in get task by name", ex);
            }
        }

        public async Task<IEnumerable<GetTaskDto>> GetUserTasksAsync(int projectId, string userName)
        {
            try
            {
                var tasks = await _unitOfWork.TaskRepository.GetUserTasksAsync(projectId, userName);
                return tasks.Select(task => new GetTaskDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Priority = task.Priority,
                    Status = task.Status,
                    Deadline = task.Deadline,
                    ProjectId = task.ProjectId,
                    ProjectName = task.Project.Name,
                    CreatorUserName = task.Creator.UserName,
                    AssigneeUserName = task.Assignee?.UserName
                });
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("error in get user tasks", ex);
            }
        }

        public async Task<GetTaskDto> UnassignTaskAsync(TaskOperationRequesterDto taskOperationRequesterDto)
        {

            try
            {
                var isProjectOwner = await IsProjectOwner(taskOperationRequesterDto.ProjectId);
                if (isProjectOwner)
                {
                    var task = await _unitOfWork.TaskRepository.GetByIdAsync(taskOperationRequesterDto.TaskId);
                    if (task == null)
                    {
                        throw new ServiceException("task not found");
                    }
                    if (task.Assignee?.UserName == taskOperationRequesterDto.UserName)
                    {
                        var request = _unitOfWork.TaskOperationRequestRepository.GetRequestsByTaskIdAsync(UserRole.Member, taskOperationRequesterDto.UserName, taskOperationRequesterDto.TaskId)
                                                        .Result.FirstOrDefault();
                        if (request != null && request.Type == RequestType.Unassign)
                        {

                            task.AssigneeId = null;
                            task.Status = Domain.Enums.TaskStatus.Pending;
                            task.UnassignRequestDescription = request.Description;
                            await _unitOfWork.TaskRepository.UpdateAsync(task.Id, task);
                            await _unitOfWork.SaveChangesAsync();

                            await _unitOfWork.TaskOperationRequestRepository.DeleteRequestAsync(UserRole.Owner, null, task.Id);
                            await _unitOfWork.SaveChangesAsync();
                            return new GetTaskDto
                            {
                                Id = task.Id,
                                Title = task.Title,
                                Description = task.Description,
                                Priority = task.Priority,
                                Status = task.Status,
                                Deadline = task.Deadline,
                                AssignRequestDescription = task.AssignRequestDescription,
                                UnassignRequestDescription = task.UnassignRequestDescription,
                                CompletionRequestDescription = task.CompletionRequestDescription,
                                ProjectId = task.ProjectId,
                                ProjectName = task.Project.Name,
                                CreatorUserName = task.Creator.UserName,
                                AssigneeUserName = taskOperationRequesterDto.UserName

                            };
                        }
                        else
                        {
                            throw new ServiceException("user unassign request not found");
                        }
                    }
                    else
                    {
                        throw new ServiceException("this user can't unassign this project");
                    }
                }
                else
                { throw new ServiceException("you don't have access permission to this section"); }
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("error in Task unassign", ex);
            }
        }

        public async Task<TaskOperationRequestDto> UpdateRequestAsync(int taskId, TaskOperationRequestDto taskOperationRequestDto)
        {
            try
            {
                var currentRequester = await CurrentRequester();
                if (currentRequester == null) {
                    throw new ServiceException("please login");
                }
                var request = _unitOfWork.TaskOperationRequestRepository.GetRequestsByTaskIdAsync(UserRole.Member, currentRequester.UserName, taskId).Result.SingleOrDefault();
                if (request != null) {
                    if (Enum.IsDefined(typeof(RequestType), taskOperationRequestDto.Type))
                    {
                        request.Type = taskOperationRequestDto.Type;
                    }
                    if (!string.IsNullOrWhiteSpace(taskOperationRequestDto.Description))
                    {
                        request.Description = taskOperationRequestDto.Description;
                    }
                    if (taskOperationRequestDto.TaskId != default(int))
                    {
                        request.TaskId = taskOperationRequestDto.TaskId;
                    }
                    if (taskOperationRequestDto.ProjectId != default(int))
                    {
                        request.TaskId = taskOperationRequestDto.ProjectId;
                    }

                    await _unitOfWork.TaskOperationRequestRepository.UpdateRequestAsync(request);
                    await _unitOfWork.SaveChangesAsync();
                    return new TaskOperationRequestDto
                    {
                        Type = request.Type,
                        Description = request.Description,
                        TaskId = request.TaskId,
                        ProjectId = request.ProjectId
                    };

                }
                else
                {
                    throw new ServiceException("request not found to update");
                }

            }catch (RepositoryException ex)
            {
               throw new ServiceException("Error in update request", ex);
            }
        }

        public async Task<GetTaskDto> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto)
        {
            try
            {
                var task = await _unitOfWork.TaskRepository.GetByIdAsync(id);
                if (task == null)
                {
                    throw new ServiceException("task not found to update");
                }
                var isProjectOwner = await IsProjectOwner(task.ProjectId);
                if(isProjectOwner)
                {
                    if(!string.IsNullOrWhiteSpace(updateTaskDto.Title))
                    {
                        task.Title = updateTaskDto.Title;
                    }
                    if(!string.IsNullOrWhiteSpace(updateTaskDto.Description))
                    {
                        task.Description = updateTaskDto.Description;
                    }
                    if(updateTaskDto.Priority != default(int))
                    {
                        task.Priority = updateTaskDto.Priority;
                    }
                    if(updateTaskDto.Deadline != null)
                    {
                        task.Deadline = updateTaskDto.Deadline;
                    }
                    if(updateTaskDto.ProjectId != default(int))
                    {
                        task.ProjectId = updateTaskDto.ProjectId;
                    }
                    await _unitOfWork.TaskRepository.UpdateAsync(id, task);
                    await _unitOfWork.SaveChangesAsync();
                    return new GetTaskDto
                    {
                        Id = task.Id,
                        Title = task.Title,
                        Description = task.Description,
                        Priority = task.Priority,
                        Status = task.Status,
                        Deadline = task.Deadline,
                        AssignRequestDescription = task.AssignRequestDescription,
                        UnassignRequestDescription = task.UnassignRequestDescription,
                        CompletionRequestDescription = task.CompletionRequestDescription,
                        ProjectId = task.ProjectId,
                        ProjectName = task.Project.Name,
                        CreatorUserName = task.Creator.UserName,
                        AssigneeUserName = task.Assignee?.UserName

                    };
                }
                else
                {
                    throw new ServiceException("you can't update this task");
                }
            }catch (RepositoryException ex)
            {
                throw new ServiceException("error in update task", ex);
            }
        }

        private async Task<User> CurrentRequester()
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userRepository.FindByNameAsync(userName);
            if (user == null)
            {
                throw new ServiceException("please login");
            }
            return user;
        }

        private async Task<bool> IsProjectOwner(int projectId)
        {
            var project = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId);
            var currentRequester = await CurrentRequester();

            if (project == null)
            {
                return false;
            }
            if (currentRequester.UserName == project.OwnerUserName)
                return true;
            return false;
        }
    }
}