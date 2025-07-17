using Microsoft.AspNetCore.Mvc;
using UserService.Application.Services;
using UserService.Domain.Entities;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionsController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService ?? throw new ArgumentNullException(nameof(subscriptionService));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSubscriptions()
    {
        try
        {
            var subscriptions = await _subscriptionService.GetAllSubscriptionsAsync();
            return Ok(subscriptions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSubscription(int id)
    {
        try
        {
            var subscription = await _subscriptionService.GetSubscriptionByIdAsync(id);
            return Ok(subscription);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionRequest request)
    {
        try
        {
            await _subscriptionService.CreateSubscriptionAsync(request.Type);
            return Ok("Subscription created successfully");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSubscription(int id, [FromBody] UpdateSubscriptionRequest request)
    {
        try
        {
            await _subscriptionService.UpdateSubscriptionAsync(id, request.Type, request.EndDate);
            return Ok("Subscription updated successfully");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSubscription(int id)
    {
        try
        {
            await _subscriptionService.DeleteSubscriptionAsync(id);
            return Ok("Subscription deleted successfully");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}

public record CreateSubscriptionRequest(SubscriptionType Type);
public record UpdateSubscriptionRequest(SubscriptionType Type, DateTime EndDate);