using System.Text.Json;
using Microsoft.Extensions.Logging;
using ProjectService.Application.DTOs;

namespace ProjectService.Application.Services;

public class UserServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserServiceClient> _logger;

    public UserServiceClient(HttpClient httpClient, ILogger<UserServiceClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<UserDto>> GetUsersBySubscriptionTypeAsync(string subscriptionType)
    {
        try
        {
            _logger.LogInformation("Getting users with subscription type: {SubscriptionType}", subscriptionType);

            var response = await _httpClient.GetAsync($"/api/users/by-subscription-type/{subscriptionType}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get users by subscription type. Status: {StatusCode}",
                    response.StatusCode);
                return Enumerable.Empty<UserDto>();
            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            var users = JsonSerializer.Deserialize<IEnumerable<UserDto>>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return users ?? Enumerable.Empty<UserDto>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error occurred while getting users by subscription type: {SubscriptionType}",
                subscriptionType);
            return Enumerable.Empty<UserDto>();
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Request timeout while getting users by subscription type: {SubscriptionType}",
                subscriptionType);
            return Enumerable.Empty<UserDto>();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex,
                "JSON deserialization error while getting users by subscription type: {SubscriptionType}",
                subscriptionType);
            return Enumerable.Empty<UserDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error occurred while getting users by subscription type: {SubscriptionType}",
                subscriptionType);
            return Enumerable.Empty<UserDto>();
        }
    }
}