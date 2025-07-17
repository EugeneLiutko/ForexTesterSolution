using UserService.Application.Repositories;
using UserService.Domain.Entities;

namespace UserService.Application.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUserRepository _userRepository;

    public SubscriptionService(ISubscriptionRepository subscriptionRepository, IUserRepository userRepository)
    {
        _subscriptionRepository =
            subscriptionRepository ?? throw new ArgumentNullException(nameof(subscriptionRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<Subscription> GetSubscriptionByIdAsync(int id)
    {
        return await _subscriptionRepository.GetByIdAsync(id)
               ?? throw new KeyNotFoundException($"Subscription with ID {id} not found");
    }

    public async Task<IEnumerable<Subscription>> GetAllSubscriptionsAsync()
    {
        return await _subscriptionRepository.GetAllAsync();
    }

    public async Task<Subscription> CreateSubscriptionAsync(SubscriptionType type)
    {
        var subscription = Subscription.CreateWithDuration(type);
        await _subscriptionRepository.AddAsync(subscription);
        return subscription;
    }

    public async Task UpdateSubscriptionAsync(int id, SubscriptionType type, DateTime endDate)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(id)
                           ?? throw new KeyNotFoundException($"Subscription with ID {id} not found");

        subscription.UpdateDetails(type, endDate);
        await _subscriptionRepository.UpdateAsync(subscription);
    }

    public async Task DeleteSubscriptionAsync(int id)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(id);
        if (subscription != null && subscription.User != null)
        {
            throw new InvalidOperationException("Cannot delete subscription that is assigned to a user");
        }

        await _subscriptionRepository.DeleteAsync(id);
    }
}