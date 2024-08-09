using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerApp.Application.DTOs;
using TaskManagerApp.Application.Interfaces;
using TaskManagerApp.Domain.Entities;
using TaskManagerApp.Infrastructure.Repositories;
using TaskManagerApp.Application.Exceptions;
using Task = System.Threading.Tasks.Task;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace TaskManagerApp.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public ProjectService( IUserRepository userRepository, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GetProjectDto?> GetProjectByIdAsync(int id)
        {
            try
            {
                return await _unitOfWork.ProjectRepository.GetByIdAsync(id);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("An error occurred while retrieving the project by ID", ex);
            }
        }

        public async Task<GetProjectDto?> GetProjectByNameAsync(string projectName)
        {
            try
            {
                return await _unitOfWork.ProjectRepository.GetByNameAsync(projectName);     
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("An error occurred while retrieving the project by name", ex);
            }
        }

        public async Task<IEnumerable<GetProjectDto>> GetAllProjectsAsync()
        {
            try
            {
                return await _unitOfWork.ProjectRepository.GetAllAsync();

            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("An error occurred while retrieving all of the projects", ex);
            }
        }

        public async Task<GetProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto)
        {
            try
            {
                var project = _mapper.Map<Project>(createProjectDto);
                project.OwnerId = CurrentRequester().Result.Id;
                await _unitOfWork.ProjectRepository.AddAsync(project);
                await _unitOfWork.SaveChangesAsync();
                return await _unitOfWork.ProjectRepository.GetByIdAsync(project.Id);

            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("An error occurred while creating the project.", ex);
            }
        }

        public async Task<GetProjectDto> UpdateProjectAsync(int projectId, UpdateProjectDto updateProjectDto)
        {
            try
            {
                var project = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId);

                if (project == null)
                {
                    throw new ServiceException("Project not found to updating");
                }

                if (CurrentRequester().Result.UserName != project.OwnerUserName)
                {
                    throw new ServiceException("you can not update this project ");
                }
                await _unitOfWork.ProjectRepository.Update(projectId, updateProjectDto);
                await _unitOfWork.SaveChangesAsync();
                return await _unitOfWork.ProjectRepository.GetByIdAsync(projectId);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("An error occurred while updating the project", ex);
            }
        }

        public async Task<IEnumerable<ProjectMemberDto>> GetProjectMembersAsync(int projectId)
        {
            try
            {
                var userName = CurrentRequester().Result.UserName;
                var project = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId);
                if(project == null)
                {
                    throw new ServiceException($"Project with id:{projectId} does not exist");
                }
                if (userName == project.OwnerUserName)
                {
                    return await _unitOfWork.ProjectRepository.GetMembers(projectId);
                }
                else
                {
                    throw new ServiceException("you can not see this project members");
                }
            }catch (RepositoryException ex)
            {
                throw new ServiceException("An error occurred while retrieving project members", ex);    
            }
        }

        public async Task<string> AddMemberToProjectAsync(int projectId, string userName)
        {
            try
            {
                var project = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId);
                if (project == null)
                {
                    throw new ServiceException("Project not found to adding new member to it");
                }

                var user = await _userRepository.FindByNameAsync(userName);
                if (user == null)
                {
                    throw new ServiceException("user not found");
                }

                var isProjectMember = await _unitOfWork.ProjectRepository.IsProjectMemberAsync(projectId, user.Id);
                if (isProjectMember)
                {
                    throw new ServiceException("user is member of project at previously");
                }

                var currentRequester = await CurrentRequester();           
                if ((currentRequester == null) || (currentRequester.UserName != project.OwnerUserName))
                {
                    throw new ServiceException("you can not add member to this project");
                }

                await _unitOfWork.ProjectRepository.AddMember(project.Id, user.Id);
                await _unitOfWork.SaveChangesAsync();
                return $"{user.UserName} added to project {project.Name} successfully";

            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("An error occurred while adding new member to the project", ex);
            }
        }


        public async Task<string> RemoveMemberFromProjectAsync(int projectId, string userName)
        {
            try
            {
                var project = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId);
                if (project == null)
                {
                    throw new ServiceException("Project not found to removing member from it");
                }

                var user = await _userRepository.FindByNameAsync(userName);
                if (user == null)
                {
                    throw new ServiceException("user not found");
                }

                var isProjectMember = await _unitOfWork.ProjectRepository.IsProjectMemberAsync(projectId, user.Id);
                if (!isProjectMember)
                {
                    throw new ServiceException("user is not project member");
                }

                var currentRequester = await CurrentRequester();
                if ((currentRequester == null) || ((currentRequester.UserName != userName) && (currentRequester.UserName != project.OwnerUserName)))
                {
                    throw new ServiceException("you can not remove this member");
                }

                await _unitOfWork.ProjectRepository.RemoveMember(project.Id, user.Id);
                await _unitOfWork.SaveChangesAsync();
                return $"{user.UserName} removed from project {project.Name} successfully";

            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("An error occurred while removing member from the project", ex);
            }
        }


        public async Task DeleteProjectAsync(int projectId)
        {
            try
            {
                var project = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId);
                var currentRequester = await CurrentRequester();

                if (project == null)
                {
                    throw new ServiceException("Project not found to deleting");
                }

                if (currentRequester.UserName != project.OwnerUserName)
                {
                    throw new ServiceException("you can not delete this project");
                }

                await _unitOfWork.ProjectRepository.Delete(project.Id);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("An error occurred while deleting the project", ex);
            }
        }

        public async Task<IEnumerable<GetProjectDto>> GetOwnedProjectsAsync()
        {
            try
            {
                var userId = CurrentRequester().Result.Id;
                var projects = await _unitOfWork.ProjectRepository.GetOwnedProjectsAsync(userId);
                return projects;
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("error in retrieving user owned projects", ex);
            }
        }

        public async Task<IEnumerable<GetProjectDto>> GetMemberProjectsAsync()
        {
            try
            {
                var userId = CurrentRequester().Result.Id;
                var projects = await _unitOfWork.ProjectRepository.GetMemberProjectsAsync(userId);
                return projects;
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("error in retrieving projects that user is member of it", ex);
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

    }
}
