using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagerApp.Domain.Entities;
using TaskManagerApp.Infrastructure.Persistence;
using TaskManagerApp.Application.Exceptions;
using Task = System.Threading.Tasks.Task;
using TaskManagerApp.Domain.Interfaces;
using TaskManagerApp.Application.DTOs;
using Microsoft.AspNetCore.Identity;

namespace TaskManagerApp.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ProjectRepository(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<GetProjectDto> GetByIdAsync(int id)
        {
            try
            {
                var projectInfo = await (from project in _context.Projects
                                         join user in _userManager.Users
                                         on project.OwnerId equals user.Id
                                         where project.Id == id && project.IsDeleted == false
                                         select new GetProjectDto
                                         {
                                             Id = project.Id,
                                             Name = project.Name,
                                             Description = project.Description,
                                             Deadline = project.Deadline,
                                             OwnerUserName = project.Owner.UserName
                                         }).SingleOrDefaultAsync();
                return projectInfo;

            }
            catch (Exception ex)
            {
                throw new RepositoryException("An error occurred while retrieving the project by ID.", ex);
            }
        }

        public async Task<GetProjectDto> GetByNameAsync(string projectName)
        {
            try
            {
                var projectInfo = await (from project in _context.Projects
                                         join user in _userManager.Users
                                         on project.OwnerId equals user.Id
                                         where project.Name == projectName && project.IsDeleted == false
                                         select new GetProjectDto
                                         {
                                             Id = project.Id,
                                             Name = project.Name,
                                             Description = project.Description,
                                             Deadline = project.Deadline,
                                             OwnerUserName = project.Owner.UserName
                                         }).SingleOrDefaultAsync();
                return projectInfo;

            }
            catch (Exception ex)
            {
                throw new RepositoryException("An error occurred while retrieving the project by name.", ex);
            }
        }

        public async Task<IEnumerable<GetProjectDto>> GetAllAsync()
        {
            try
            {
                var projectsList = await (from project in _context.Projects
                                         join user in _userManager.Users
                                         on project.OwnerId equals user.Id
                                         where project.IsDeleted == false
                                         select new GetProjectDto
                                         {
                                             Id = project.Id,
                                             Name = project.Name,
                                             Description = project.Description,
                                             Deadline = project.Deadline,
                                             OwnerUserName = project.Owner.UserName
                                         }).ToListAsync();
                return projectsList;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An error occurred while retrieving all of the projects.", ex);
            }
        }


        public async Task AddAsync(Project project)
        {
            try
            {
                 await _context.Projects.AddAsync(project);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An error occurred while adding the project.", ex);
            }
        }

        public async Task Update(int id, UpdateProjectDto updateProjectDto)
        {
            try
            {
               var project = await _context.Projects.Where(p => p.Id == id).SingleOrDefaultAsync();
                if(!string.IsNullOrWhiteSpace(updateProjectDto.Name))
                    project.Name = updateProjectDto.Name;

                if(!string.IsNullOrWhiteSpace(updateProjectDto.Description))
                    project.Description = updateProjectDto.Description;

                if(updateProjectDto.DeadLine > DateTime.Now)
                    project.Deadline = updateProjectDto.DeadLine;

                _context.Projects.Update(project);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An error occurred while updating the project.", ex);
            }
        }

        public async Task Delete(int projectId)
        {
            try
            {
                var project = await _context.Projects.Where(p => p.Id == projectId && p.IsDeleted == false).SingleOrDefaultAsync();
                project.IsDeleted = true;
                _context.Projects.Update(project);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An error occurred while deleting the project.", ex);
            }
        }

        public async Task<IEnumerable<ProjectMemberDto>> GetMembers(int projectId)
        {
            try
            {
                var members = await (from userProject in _context.UserProjects
                                     join user in _userManager.Users
                                     on userProject.UserId equals user.Id
                                     where userProject.ProjectId == projectId
                                     select new ProjectMemberDto
                                     {
                                         userName = user.UserName,
                                         Email = user.Email,
                                         HasCurrentTask = _context.Tasks.Any(t => t.AssigneeId == user.Id && t.ProjectId == projectId && (t.Status == Domain.Enums.TaskStatus.Pending || t.Status == Domain.Enums.TaskStatus.InProgress))
                                     }).ToListAsync();

                return members;


            }
            catch (Exception ex)
            {
                throw new RepositoryException("error in retrieving project members", ex);
            }
        }

        public async Task AddMember(int projectId, string userId)
        {
            try
            {
                var project = await _context.Projects.Include(p => p.UserProjects).SingleOrDefaultAsync(p => p.Id == projectId);
                if (!project.UserProjects.Any(up => up.UserId == userId))
                {
                    project.UserProjects.Add(new UserProject { UserId = userId, ProjectId = projectId });
                }
            }
            catch (Exception ex)
            {
                throw new RepositoryException("error in adding member to project", ex);

            }
        }

        public async Task RemoveMember(int projectId , string userId)
        {
            try
            {
                var project = await _context.Projects.Include(p => p.UserProjects).SingleOrDefaultAsync(p => p.Id == projectId);
                var userProject = project.UserProjects.FirstOrDefault(up => up.UserId == userId);
                project.UserProjects.Remove(userProject);

            }
            catch (Exception ex)
            {
                throw new RepositoryException("error in removing member from project", ex);

            }
        }

        public async Task<IEnumerable<GetProjectDto>> GetOwnedProjectsAsync(string userId)
        {
            try
            {
                return await (from project in _context.Projects
                              join user in _userManager.Users
                              on project.OwnerId equals user.Id
                              where project.OwnerId == userId && project.IsDeleted == false
                              select new GetProjectDto
                              {
                                  Id = project.Id,
                                  Name = project.Name,
                                  Description = project.Description,
                                  Deadline = project.Deadline,
                                  OwnerUserName = project.Owner.UserName
                              }).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An error occurred while retrieving Owned projects.", ex);
            }
        }

        public async Task<IEnumerable<GetProjectDto>> GetMemberProjectsAsync(string userId)
        {
            try
            {
                return await (from project in _context.Projects
                              join userProject in _context.UserProjects on project.Id equals userProject.ProjectId
                              where userProject.UserId == userId && project.IsDeleted == false
                              select new GetProjectDto
                              {
                                  Id = project.Id,
                                  Name = project.Name,
                                  Description = project.Description,
                                  Deadline = project.Deadline,
                                  OwnerUserName = project.Owner.UserName
                              }).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An error occurred while retrieving Member projects.", ex);
            }
        }


        public async Task<bool> IsProjectMemberAsync(int projectId, string userId)
        {
            try
            {
                bool isMember = await _context.UserProjects
                                    .AnyAsync(up => up.ProjectId == projectId && up.UserId == userId);

                return isMember;
            }
            catch (Exception)
            {
               return false;
            }
        }

    }

}


