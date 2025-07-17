using Microsoft.AspNetCore.Mvc;
using ProjectService.Application.DTOs;
using ProjectService.Application.Services;

namespace ProjectService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserSettingsController : ControllerBase
{
    private readonly IUserSettingsService _userSettingsService;
    private readonly ILogger<UserSettingsController> _logger;

    public UserSettingsController(IUserSettingsService userSettingsService, ILogger<UserSettingsController> logger)
    {
        _userSettingsService = userSettingsService ?? throw new ArgumentNullException(nameof(userSettingsService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get user settings by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserSettingsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUserSettings(string id)
    {
        try
        {
            var userSettings = await _userSettingsService.GetUserSettingsByIdAsync(id);
            return Ok(userSettings);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user settings by ID: {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while retrieving user settings" });
        }
    }

    /// <summary>
    /// Get user settings by user ID
    /// </summary>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(UserSettingsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUserSettingsByUserId(int userId)
    {
        try
        {
            var userSettings = await _userSettingsService.GetUserSettingsByUserIdAsync(userId);
            return Ok(userSettings);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user settings by user ID: {UserId}", userId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while retrieving user settings" });
        }
    }

    /// <summary>
    /// Get all user settings
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserSettingsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUserSettings()
    {
        try
        {
            var userSettings = await _userSettingsService.GetAllUserSettingsAsync();
            return Ok(userSettings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all user settings");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while retrieving user settings" });
        }
    }

    /// <summary>
    /// Create new user settings
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(UserSettingsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUserSettings([FromBody] CreateUserSettingsRequest request)
    {
        try
        {
            var userSettings = await _userSettingsService.CreateUserSettingsAsync(request);
            return CreatedAtAction(nameof(GetUserSettings), new { id = userSettings.Id }, userSettings);
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { message = "Request cannot be null" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user settings");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while creating user settings" });
        }
    }

    /// <summary>
    /// Update user settings
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UserSettingsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUserSettings(string id, [FromBody] UpdateUserSettingsRequest request)
    {
        try
        {
            var userSettings = await _userSettingsService.UpdateUserSettingsAsync(id, request);
            return Ok(userSettings);
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(new { message = "Request cannot be null" });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user settings with ID: {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while updating user settings" });
        }
    }

    /// <summary>
    /// Delete user settings
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUserSettings(string id)
    {
        try
        {
            await _userSettingsService.DeleteUserSettingsAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user settings with ID: {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while deleting user settings" });
        }
    }
}