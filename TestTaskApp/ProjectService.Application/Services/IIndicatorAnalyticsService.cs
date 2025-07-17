using ProjectService.Application.DTOs;

namespace ProjectService.Application.Services;

public interface IIndicatorAnalyticsService
{
    Task<PopularIndicatorsResponse> GetTopIndicatorsBySubscriptionTypeAsync(string subscriptionType);
}