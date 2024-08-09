using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagerApp.Api.Filters;
using TaskManagerApp.Application.DTOs;
using TaskManagerApp.Application.Exceptions;
using TaskManagerApp.Application.Interfaces;
using System.Threading.Tasks;

/// <summary>
/// Controller for managing projects.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    /// <summary>
    /// Gets a project by its ID.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <returns>The project details.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProjectById(int id)
    {
        try
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return Ok(project);
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }

    /// <summary>
    /// Gets a project by its name.
    /// </summary>
    /// <param name="projectName">The project name.</param>
    /// <returns>The project details.</returns>
    [HttpGet("by-name/{projectName}")]
    public async Task<IActionResult> GetProjectByName(string projectName)
    {
        try
        {
            var project = await _projectService.GetProjectByNameAsync(projectName);
            if (project == null)
            {
                return NotFound();
            }
            return Ok(project);
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }

    /// <summary>
    /// Gets all projects.
    /// </summary>
    /// <returns>List of all projects.</returns>
    [HttpGet("all")]
    public async Task<IActionResult> GetAllProjects()
    {
        try
        {
            var allProjects = await _projectService.GetAllProjectsAsync();
            if (!allProjects.Any())
            {
                return NotFound();
            }
            return Ok(allProjects);
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }

    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="createProjectDto">The project creation data.</param>
    /// <returns>HTTP status code.</returns>
    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto createProjectDto)
    {
        try
        {
            var project = await _projectService.CreateProjectAsync(createProjectDto);
            var response = new
            {
                Message = "project created successfully",
                Project = project
            };
            return new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status201Created
            };
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <param name="updateProjectDto">The project update data.</param>
    /// <returns>HTTP status code.</returns>
    [HttpPut("{id}")]
    [ValidateModel]
    public async Task<IActionResult> UpdateProject(int id, [FromBody] UpdateProjectDto updateProjectDto)
    {
        try
        {
            var project = await _projectService.UpdateProjectAsync(id, updateProjectDto);
            return Ok(project);
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }


    /// <summary>
    /// Get all members of a owned project
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <returns>HTTP status code.</returns>
    [HttpGet("get-members")]
    public async Task<IActionResult> GetOwnedProjectMembers([FromQuery] int projectId)
    {
        try
        {
           var projectMembers = await _projectService.GetProjectMembersAsync(projectId);
            if (!projectMembers.Any())
            {
                return NotFound();
            }
            return Ok(projectMembers);
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }


    /// <summary>
    /// Adds a member to a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="userName">The username of the member to add.</param>
    /// <returns>HTTP status code.</returns>
    [HttpPost("add-member")]
    public async Task<IActionResult> AddMemberToProject([FromBody] int projectId, string userName)
    {
        try
        {
            var result = await _projectService.AddMemberToProjectAsync(projectId, userName);
            return new ObjectResult(result) { 
                 StatusCode = StatusCodes.Status201Created
            };
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }

    /// <summary>
    /// Removes a member from a project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="userName">The username of the member to remove.</param>
    /// <returns>HTTP status code.</returns>
    [HttpDelete("remove-member")]
    public async Task<IActionResult> RemoveMemberFromProject([FromBody] int projectId, string userName)
    {
        try
        {
            var result = await _projectService.RemoveMemberFromProjectAsync(projectId, userName);
            return new ObjectResult(result)
            {
                StatusCode = StatusCodes.Status204NoContent
            };
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }

    /// <summary>
    /// Deletes a project.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <returns>HTTP status code.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        try
        {
            await _projectService.DeleteProjectAsync(id);
            return NoContent();
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }

    /// <summary>
    /// Gets the projects owned by the current user.
    /// </summary>
    /// <returns>List of owned projects.</returns>
    [HttpGet("owned")]
    public async Task<IActionResult> GetOwnedProjects()
    {
        try
        {
            var projects = await _projectService.GetOwnedProjectsAsync();
            if (!projects.Any())
            {
                return NotFound();
            }
            return Ok(projects);
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }

    /// <summary>
    /// Gets the projects the current user is a member of.
    /// </summary>
    /// <returns>List of member projects.</returns>
    [HttpGet("member")]
    public async Task<IActionResult> GetMemberProjects()
    {
        try
        {
            var projects = await _projectService.GetMemberProjectsAsync();
            if (!projects.Any())
            {
                return NotFound();
            }
            return Ok(projects);
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }
}
