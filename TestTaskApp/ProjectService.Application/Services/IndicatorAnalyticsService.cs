using Microsoft.Extensions.Logging;
using ProjectService.Application.DTOs;
using ProjectService.Application.Repositories;

namespace ProjectService.Application.Services;
public class IndicatorAnalyticsService : IIndicatorAnalyticsService
{
    private readonly IProjectRepository _projectRepository;
    private readonly UserServiceClient _userServiceClient;
    private readonly ILogger<IndicatorAnalyticsService> _logger;

    public IndicatorAnalyticsService(
        IProjectRepository projectRepository, 
        UserServiceClient userServiceClient,
        ILogger<IndicatorAnalyticsService> logger)
    {
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        _userServiceClient = userServiceClient ?? throw new ArgumentNullException(nameof(userServiceClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PopularIndicatorsResponse> GetTopIndicatorsBySubscriptionTypeAsync(string subscriptionType)
    {
        try
        {
            _logger.LogInformation("Getting top indicators for subscription type: {SubscriptionType}", subscriptionType);

            var validTypes = new[] { "Free", "Trial", "Super" };
            if (!validTypes.Contains(subscriptionType, StringComparer.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Invalid subscription type: {SubscriptionType}", subscriptionType);
                return new PopularIndicatorsResponse(new List<PopularIndicatorDto>());
            }

            // 1. Отримати користувачів з вибраним типом підписки з UserService
            var users = await _userServiceClient.GetUsersBySubscriptionTypeAsync(subscriptionType);
            
            var userIds = users.Select(u => u.Id).ToList();

            if (!userIds.Any())
            {
                _logger.LogWarning("No users found with subscription type: {SubscriptionType}", subscriptionType);
                return new PopularIndicatorsResponse(new List<PopularIndicatorDto>());
            }

            _logger.LogInformation("Found {UserCount} users with subscription type: {SubscriptionType}", userIds.Count, subscriptionType);

            // 2. Отримати всі проекти цих користувачів
            var projects = await _projectRepository.GetProjectsByUserIdsAsync(userIds);

            // 3. Порахувати індикатори в Dictionary
            var indicatorCounts = new Dictionary<string, int>();

            foreach (var project in projects)
            {
                foreach (var chart in project.Charts)
                {
                    foreach (var indicator in chart.Indicators)
                    {
                        var indicatorName = indicator.Name.ToString();
                        indicatorCounts[indicatorName] = indicatorCounts.GetValueOrDefault(indicatorName, 0) + 1;
                    }
                }
            }

            // 4. Повернути топ 3
            var topIndicators = indicatorCounts
                .OrderByDescending(kvp => kvp.Value)
                .Take(3)
                .Select(kvp => new PopularIndicatorDto(kvp.Key, kvp.Value))
                .ToList();

            _logger.LogInformation("Found {IndicatorCount} unique indicators for subscription type: {SubscriptionType}. Top indicators: {TopIndicators}", 
                indicatorCounts.Count, subscriptionType, string.Join(", ", topIndicators.Select(i => $"{i.Name}({i.Used})")));

            return new PopularIndicatorsResponse(topIndicators);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting top indicators for subscription type: {SubscriptionType}", subscriptionType);
            return new PopularIndicatorsResponse(new List<PopularIndicatorDto>());
        }
    }
}