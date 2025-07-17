using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Infrastructure.Persistence;
using UserService.Infrastructure.Repositories;

namespace UserService.Tests;

public class SubscriptionRepositoryTests
{
    private readonly UserDbContext _context;
    private readonly SubscriptionRepository _subscriptionRepository;

    public SubscriptionRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new UserDbContext(options);
        _subscriptionRepository = new SubscriptionRepository(_context);
    }

    [Fact]
    public async Task DeleteAsync_WhenSubscriptionHasUser_ThrowsInvalidOperationException()
    {
        // Arrange
        var subscription = new Subscription(SubscriptionType.Free, DateTime.UtcNow, SubscriptionConstants.MAX_SUBSCRIPTION_DATE);
        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();

        var user = new User("John Doe", "john@example.com", subscription.Id);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _subscriptionRepository.DeleteAsync(subscription.Id));

        Assert.Contains("Cannot delete subscription that is assigned to a user", exception.Message);
        
        var subscriptionExists = await _context.Subscriptions.FindAsync(subscription.Id);
        Assert.NotNull(subscriptionExists);
    }

    [Fact]
    public async Task DeleteAsync_WhenSubscriptionHasNoUser_DeletesSuccessfully()
    {
        // Arrange
        var subscription = new Subscription(SubscriptionType.Trial, DateTime.UtcNow, DateTime.UtcNow.AddMonths(SubscriptionConstants.TRIAL_DURATION_MONTHS));
        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();
        var subscriptionId = subscription.Id;

        // Act
        await _subscriptionRepository.DeleteAsync(subscriptionId);

        // Assert
        var deletedSubscription = await _context.Subscriptions.FindAsync(subscriptionId);
        Assert.Null(deletedSubscription);
    }

    [Fact]
    public async Task DeleteAsync_WhenSubscriptionNotExists_DoesNotThrow()
    {
        // Arrange
        var nonExistentId = 999;

        // Act & Assert
        await _subscriptionRepository.DeleteAsync(nonExistentId);
        
        Assert.True(true);
    }

    [Fact]
    public async Task DeleteAsync_WhenMultipleUsersExist_OnlyChecksRelevantUser()
    {
        // Arrange
        var subscription1 = new Subscription(SubscriptionType.Free, DateTime.UtcNow, SubscriptionConstants.MAX_SUBSCRIPTION_DATE);
        var subscription2 = new Subscription(SubscriptionType.Trial, DateTime.UtcNow, DateTime.UtcNow.AddMonths(SubscriptionConstants.TRIAL_DURATION_MONTHS));
        
        _context.Subscriptions.AddRange(subscription1, subscription2);
        await _context.SaveChangesAsync();

        var user = new User("John Doe", "john@example.com", subscription1.Id);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act 
        await _subscriptionRepository.DeleteAsync(subscription2.Id);

        // Assert
        var deletedSubscription = await _context.Subscriptions.FindAsync(subscription2.Id);
        Assert.Null(deletedSubscription);
        
        var remainingSubscription = await _context.Subscriptions.FindAsync(subscription1.Id);
        Assert.NotNull(remainingSubscription);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}