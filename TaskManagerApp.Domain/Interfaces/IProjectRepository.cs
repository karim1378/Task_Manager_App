using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagerApp.Application.DTOs;
using TaskManagerApp.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace TaskManagerApp.Domain.Interfaces;

public interface IProjectRepository
{
    Task<GetProjectDto> GetByIdAsync(int id);
    Task<GetProjectDto> GetByNameAsync(string projectName);
    Task<IEnumerable<GetProjectDto>> GetAllAsync();
    Task AddAsync(Project project);
    Task Update(int id, UpdateProjectDto updateProjectDto);
    Task Delete(int projectId);// Logical delete
    Task<IEnumerable<ProjectMemberDto>> GetMembers(int projectId);
    Task AddMember(int projectId, string userId);
    Task RemoveMember(int projectId, string userId);
    Task<IEnumerable<GetProjectDto>> GetOwnedProjectsAsync(string userId);
    Task<IEnumerable<GetProjectDto>> GetMemberProjectsAsync(string userId);
    Task<bool> IsProjectMemberAsync(int projectId, string userId);
}
