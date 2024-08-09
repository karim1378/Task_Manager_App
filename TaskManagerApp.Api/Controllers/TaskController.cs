using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagerApp.Api.Filters;
using TaskManagerApp.Application.DTOs;
using TaskManagerApp.Application.Exceptions;
using TaskManagerApp.Application.Interfaces;
using TaskManagerApp.TaskManagerApp.Application.DTOs;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    /// <summary>
    /// Gets a task by ID.
    /// </summary>
    /// <param name="id">The task ID.</param>
    /// <returns>HTTP status code and the task.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaskById(int id)
    {
        try
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }

    /// <summary>
    /// Gets a task by name.
    /// </summary>
    /// <param name="name">The task name.</param>
    /// <returns>HTTP status code and the task.</returns>
    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetTaskByName(string name)
    {
        try
        {
            var task = await _taskService.GetTaskByNameAsync(name);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }

    /// <summary>
    /// Deletes a task by ID.
    /// </summary>
    /// <param name="id">The task ID.</param>
    /// <returns>HTTP status code.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        try
        {
            await _taskService.DeleteTaskAsync(id);
            return NoContent();
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }

    /// <summary>
    /// Creates a new task.
    /// </summary>
    /// <param name="createTaskDto">The task creation data.</param>
    /// <returns>HTTP status code and the created task.</returns>
    [HttpPost]
    [ValidateModel]
    public async Task<IActionResult> CreateTask(CreateTaskDto createTaskDto)
    {
        try
        {
            var task = await _taskService.CreateTaskAsync(createTaskDto);
            var response = new
            {
                Message = "Task created successfully",
                Task = task
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
    /// Updates an existing task.
    /// </summary>
    /// <param name="id">The task ID.</param>
    /// <param name="updateTaskDto">The task update data.</param>
    /// <returns>HTTP status code and the updated task.</returns>
    [HttpPut("{id}")]
    [ValidateModel]
    public async Task<IActionResult> UpdateTask(int id, UpdateTaskDto updateTaskDto)
    {
        try
        {
            var task = await _taskService.UpdateTaskAsync(id, updateTaskDto);
            return Ok(task);
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }

    /// <summary>
    /// Gets tasks assigned to a specific user in a specific project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <param name="userName">The user's name.</param>
    /// <returns>HTTP status code and the list of tasks.</returns>
    [HttpGet("user/{projectId}/{userName}")]
    public async Task<IActionResult> GetUserTasks(int projectId, string userName)
    {
        try
        {
            var tasks = await _taskService.GetUserTasksAsync(projectId, userName);
            if (tasks == null || !tasks.Any())
            {
                return NotFound();
            }
            return Ok(tasks);
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }

    /// <summary>
    /// Gets all tasks of a specific project.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <returns>HTTP status code and the list of tasks.</returns>
    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetProjectTasks(int projectId)
    {
        try
        {
            var tasks = await _taskService.GetProjectTasksAsync(projectId);
            if (tasks == null || !tasks.Any())
            {
                return NotFound();
            }
            return Ok(tasks);
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }

    /// <summary>
    /// Adds a request for a task.
    /// </summary>
    /// <param name="taskOperationRequestDto">The request data.</param>
    /// <returns>HTTP status code and the created request.</returns>
    [HttpPost("request")]
    [ValidateModel]
    public async Task<IActionResult> AddRequest(TaskOperationRequestDto taskOperationRequestDto)
    {
        try
        {
            var request = await _taskService.AddRequestAsync(taskOperationRequestDto);
            var response = new
            {
                Message = "Request added successfully",
                Request = request
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
    /// Deletes a request by task ID.
    /// </summary>
    /// <param name="taskId">The task ID.</param>
    /// <returns>HTTP status code.</returns>
    [HttpDelete("request/{taskId}")]
    public async Task<IActionResult> DeleteRequest(int taskId)
    {
        try
        {
            await _taskService.DeleteRequestAsync(taskId);
            return NoContent();
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }

    /// <summary>
    /// Updates an existing request.
    /// </summary>
    /// <param name="taskId">The task ID.</param>
    /// <param name="taskOperationRequestDto">The request update data.</param>
    /// <returns>HTTP status code and the updated request.</returns>
    [HttpPut("request/{taskId}")]
    [ValidateModel]
    public async Task<IActionResult> UpdateRequest(int taskId, TaskOperationRequestDto taskOperationRequestDto)
    {
        try
        {
            var request = await _taskService.UpdateRequestAsync(taskId, taskOperationRequestDto);
            return Ok(request);
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }

    /// <summary>
    /// Gets project all requests.
    /// </summary>
    /// <param name="projectId">The project ID.</param>
    /// <returns>HTTP status code and the list of requests.</returns>
    [HttpGet("requests/{projectId}")]
    public async Task<IActionResult> GetProjectAllRequests(int projectId)
    {
        try
        {
            var requests = await _taskService.GetRequestsByProjectIdAsync(projectId);
            if (requests == null || !requests.Any())
            {
                return NotFound();
            }
            return Ok(requests);
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }

    /// <summary>
    /// Gets requests by task ID.
    /// </summary>
    /// <param name="taskId">The task ID.</param>
    /// <returns>HTTP status code and the list of requests.</returns>
    [HttpGet("requests/task/{taskId}")]
    public async Task<IActionResult> GetRequestsByTaskId(int taskId)
    {
        try
        {
            var requests = await _taskService.GetRequestsByTaskIdAsync(taskId);
            if (requests == null || !requests.Any())
            {
                return NotFound();
            }
            return Ok(requests);
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }

    /// <summary>
    /// Assigns a task to a user.
    /// </summary>
    /// <param name="taskOperationRequesterDto">The task assignment data.</param>
    /// <returns>HTTP status code and the assigned task.</returns>
    [HttpPost("assign")]
    [ValidateModel]
    public async Task<IActionResult> AssignTask([FromBody] TaskOperationRequesterDto taskOperationRequesterDto)
    {
        try
        {
            var task = await _taskService.AssignTaskAsync(taskOperationRequesterDto);
            return Ok(task);
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }

    /// <summary>
    /// Unassigns a task from a user.
    /// </summary>

    /// <param name="taskOperationRequesterDto">The task unassignment data.</param>
    /// <returns>HTTP status code and the unassigned task.</returns>
    [HttpPost("unassign")]
    [ValidateModel]
    public async Task<IActionResult> UnassignTask([FromBody] TaskOperationRequesterDto taskOperationRequesterDto)
    {
        try
        {
            var task = await _taskService.UnassignTaskAsync(taskOperationRequesterDto);
            return Ok(task);
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " + ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }

    /// <summary>
    /// Completes a task.
    /// </summary>
    /// <param name="taskOperationRequesterDto">The task completion data.</param>
    /// <returns>HTTP status code and the completed task.</returns>
    [HttpPost("complete")]
    [ValidateModel]
    public async Task<IActionResult> CompleteTask([FromBody] TaskOperationRequesterDto taskOperationRequesterDto)
    {
        try
        {
            var task = await _taskService.CompleteTaskAsync(taskOperationRequesterDto);
            return Ok(task);
        }
        catch (ServiceException ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Service Exception : " +ex.Message + "  ,  " + "Repository Exception : " + ex.InnerException);
        }
    }
}
