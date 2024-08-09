using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerApp.Application.DTOs;
using TaskManagerApp.Domain.Entities;
using Task = System.Threading.Tasks.Task;
namespace TaskManagerApp.Application.Interfaces
{
    public interface IProjectService
    {
        Task<GetProjectDto?> GetProjectByIdAsync(int projectId);
        Task<GetProjectDto?> GetProjectByNameAsync(string projectName);
        Task<IEnumerable<GetProjectDto>> GetAllProjectsAsync();
        Task<GetProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto);
        Task<GetProjectDto> UpdateProjectAsync(int projectId, UpdateProjectDto updateProjectDto);
        Task DeleteProjectAsync(int projectId);
        Task<IEnumerable<ProjectMemberDto>> GetProjectMembersAsync(int projectId);
        Task<string> AddMemberToProjectAsync(int projectId, string userName);
        Task<string> RemoveMemberFromProjectAsync(int projectId, string userName);
        Task<IEnumerable<GetProjectDto>> GetOwnedProjectsAsync();
        Task<IEnumerable<GetProjectDto>> GetMemberProjectsAsync();

    }
}


