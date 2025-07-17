using Microsoft.AspNetCore.Mvc;
using UserService.Application.DTOs;
using UserService.Application.Services;
using UserService.Domain.Entities;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;


    public UsersController(IUserService userService,ILogger<UsersController> logger )
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();
        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        await _userService.CreateUserAsync(request.Name, request.Email);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        await _userService.UpdateUserAsync(id, request.Name, request.Email);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await _userService.DeleteUserAsync(id);
        return Ok();
    }

    /// <summary>
    /// Get users by subscription type
    /// </summary>
    /// <param name="subscriptionType">Subscription type: Free, Trial, or Super</param>
    /// <returns>List of users with specified subscription type</returns>
    [HttpGet("by-subscription-type/{subscriptionType}")]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUsersBySubscriptionType(string subscriptionType)
    {
        try
        {
            _logger.LogInformation("Getting users with subscription type: {SubscriptionType}", subscriptionType);
            
            if (!Enum.TryParse<SubscriptionType>(subscriptionType, true, out var subType))
            {
                return BadRequest(new { message = "Invalid subscription type. Valid values: Free, Trial, Super" });
            }

            var users = await _userService.GetUsersBySubscriptionTypeAsync(subType);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users by subscription type: {SubscriptionType}", subscriptionType);
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while retrieving users" });
        }
    }
    
    /// <summary>
    /// Get all users
    /// </summary>
    /// <returns>List of all users</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            _logger.LogInformation("Getting all users");
            
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "An error occurred while retrieving users" });
        }
    }
}

public record CreateUserRequest(string Name, string Email);
public record UpdateUserRequest(string Name, string Email);