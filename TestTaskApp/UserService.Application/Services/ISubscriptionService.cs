using UserService.Domain.Entities;

namespace UserService.Application.Services;

public interface ISubscriptionService
{
    Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync();
    Task<Subscription?> GetSubscriptionByIdAsync(int id);
    Task<Subscription> CreateSubscriptionAsync(SubscriptionType type);
    Task UpdateSubscriptionAsync(int id, SubscriptionType type, DateTime endDate);
    Task DeleteSubscriptionAsync(int id);
}