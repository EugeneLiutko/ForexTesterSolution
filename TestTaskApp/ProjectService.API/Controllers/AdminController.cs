using Microsoft.AspNetCore.Mvc;
using ProjectService.Infrastructure.Persistence;

namespace ProjectService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly MongoDbContext _mongoContext;
    private readonly ILogger<AdminController> _logger;

    public AdminController(MongoDbContext mongoContext, ILogger<AdminController> logger)
    {
        _mongoContext = mongoContext ?? throw new ArgumentNullException(nameof(mongoContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Отримати статус MongoDB
    /// </summary>
    [HttpGet("database-status")]
    public async Task<IActionResult> GetDatabaseStatus()
    {
        try
        {
            var status = await DatabaseInitializer.GetDatabaseStatusAsync(_mongoContext);
            var isInitialized = await DatabaseInitializer.IsDatabaseInitializedAsync(_mongoContext);
            var projectsStats = await DatabaseInitializer.GetProjectsStatsByUserAsync(_mongoContext);

            return Ok(new
            {
                Status = status,
                IsInitialized = isInitialized,
                ProjectsStatsByUser = projectsStats
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting database status");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while getting database status" });
        }
    }

    /// <summary>
    /// Перестворити тестові дані
    /// </summary>
    [HttpPost("recreate-test-data")]
    public async Task<IActionResult> RecreateTestData()
    {
        try
        {
            if (!HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
            {
                return BadRequest(new { message = "This endpoint is only available in Development environment" });
            }

            await DatabaseInitializer.RecreateTestDataAsync(_mongoContext);
            
            var status = await DatabaseInitializer.GetDatabaseStatusAsync(_mongoContext);
            return Ok(new { message = "Test data recreated successfully", status });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recreating test data");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while recreating test data" });
        }
    }

    /// <summary>
    /// Очистити всі дані в MongoDB
    /// </summary>
    [HttpDelete("clear-all-data")]
    public async Task<IActionResult> ClearAllData()
    {
        try
        {
            if (!HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
            {
                return BadRequest(new { message = "This endpoint is only available in Development environment" });
            }

            await DatabaseInitializer.ClearAllDataAsync(_mongoContext);
            return Ok(new { message = "All data cleared successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing all data");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while clearing data" });
        }
    }
}