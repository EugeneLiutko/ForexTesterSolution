using Microsoft.AspNetCore.Mvc;
using ProjectService.Application.DTOs;
using ProjectService.Application.Services;

namespace ProjectService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(IProjectService projectService, ILogger<ProjectsController> logger)
    {
        _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get project by ID
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <returns>Project details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProject(string id)
    {
        try
        {
            _logger.LogInformation("Getting project with ID: {ProjectId}", id);

            var project = await _projectService.GetProjectByIdAsync(id);
            return Ok(project);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid project ID format: {ProjectId}. Error: {Error}", id, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Project not found: {ProjectId}. Error: {Error}", id, ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting project with ID: {ProjectId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while retrieving the project" });
        }
    }

    /// <summary>
    /// Get all projects for a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>List of user's projects</returns>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<ProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProjectsByUser(int userId)
    {
        try
        {
            _logger.LogInformation("Getting projects for user: {UserId}", userId);

            var projects = await _projectService.GetProjectsByUserIdAsync(userId);
            return Ok(projects);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid user ID: {UserId}. Error: {Error}", userId, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting projects for user: {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while retrieving projects" });
        }
    }

    /// <summary>
    /// Get all projects
    /// </summary>
    /// <returns>List of all projects</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllProjects()
    {
        try
        {
            _logger.LogInformation("Getting all projects");

            var projects = await _projectService.GetAllProjectsAsync();
            return Ok(projects);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all projects");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while retrieving projects" });
        }
    }

    /// <summary>
    /// Create a new project
    /// </summary>
    /// <param name="request">Project creation request</param>
    /// <returns>Created project</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
    {
        try
        {
            _logger.LogInformation("Creating project for user: {UserId} with name: {ProjectName}",
                request?.UserId, request?.Name);

            var project = await _projectService.CreateProjectAsync(request);

            return CreatedAtAction(
                nameof(GetProject),
                new { id = project.Id },
                project);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogWarning("Null request provided. Error: {Error}", ex.Message);
            return BadRequest(new { message = "Request cannot be null" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid project data. Error: {Error}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating project");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while creating the project" });
        }
    }

    /// <summary>
    /// Update an existing project
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <param name="request">Project update request</param>
    /// <returns>Updated project</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProject(string id, [FromBody] UpdateProjectRequest request)
    {
        try
        {
            _logger.LogInformation("Updating project with ID: {ProjectId}", id);

            var project = await _projectService.UpdateProjectAsync(id, request);
            return Ok(project);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogWarning("Null request provided for project update: {ProjectId}. Error: {Error}", id, ex.Message);
            return BadRequest(new { message = "Request cannot be null" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid data for project update: {ProjectId}. Error: {Error}", id, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Project not found for update: {ProjectId}. Error: {Error}", id, ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating project with ID: {ProjectId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while updating the project" });
        }
    }

    /// <summary>
    /// Delete a project
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProject(string id)
    {
        try
        {
            _logger.LogInformation("Deleting project with ID: {ProjectId}", id);

            await _projectService.DeleteProjectAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid project ID format for deletion: {ProjectId}. Error: {Error}", id, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Project not found for deletion: {ProjectId}. Error: {Error}", id, ex.Message);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting project with ID: {ProjectId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while deleting the project" });
        }
    }
}