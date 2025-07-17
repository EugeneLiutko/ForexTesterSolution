using Microsoft.AspNetCore.Mvc;
using ProjectService.Application.DTOs;
using ProjectService.Application.Services;

namespace ProjectService.Controllers;

[ApiController]
[Route("api")]
public class AnalyticsController : ControllerBase
{
    private readonly IIndicatorAnalyticsService _analyticsService;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(IIndicatorAnalyticsService analyticsService, ILogger<AnalyticsController> logger)
    {
        _analyticsService = analyticsService ?? throw new ArgumentNullException(nameof(analyticsService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get top 3 most used indicator names by subscription type
    /// </summary>
    /// <param name="subscriptionType">Subscription type: Free, Trial, or Super</param>
    /// <returns>Top 3 indicators with usage count</returns>
    [HttpGet("popularIndicators/{subscriptionType}")]
    [ProducesResponseType(typeof(PopularIndicatorsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPopularIndicators(string subscriptionType)
    {
        try
        {
            _logger.LogInformation("Getting popular indicators for subscription type: {SubscriptionType}",
                subscriptionType);

            if (string.IsNullOrWhiteSpace(subscriptionType))
            {
                return BadRequest(new { message = "Subscription type is required" });
            }

            var validTypes = new[] { "Free", "Trial", "Super" };
            if (!validTypes.Contains(subscriptionType, StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "Invalid subscription type. Valid values: Free, Trial, Super" });
            }

            var result = await _analyticsService.GetTopIndicatorsBySubscriptionTypeAsync(subscriptionType);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting popular indicators for subscription type: {SubscriptionType}",
                subscriptionType);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "An error occurred while retrieving popular indicators" });
        }
    }
}