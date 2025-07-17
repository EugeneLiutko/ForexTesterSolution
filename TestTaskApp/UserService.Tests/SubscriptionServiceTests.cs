using Moq;
using UserService.Application.Repositories;
using UserService.Application.Services;
using UserService.Domain.Entities;

namespace UserService.Tests;

public class SubscriptionServiceTests
{
    private readonly Mock<ISubscriptionRepository> _mockSubscriptionRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly SubscriptionService _subscriptionService;

    public SubscriptionServiceTests()
    {
        _mockSubscriptionRepository = new Mock<ISubscriptionRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _subscriptionService = new SubscriptionService(_mockSubscriptionRepository.Object, _mockUserRepository.Object);
    }

    [Fact]
    public async Task GetSubscriptionByIdAsync_WhenSubscriptionExists_ReturnsSubscription()
    {
        // Arrange
        var subscriptionId = 1;
        var expectedSubscription = new Subscription(SubscriptionType.Free, DateTime.UtcNow,
            SubscriptionConstants.MAX_SUBSCRIPTION_DATE);
        _mockSubscriptionRepository.Setup(x => x.GetByIdAsync(subscriptionId))
            .ReturnsAsync(expectedSubscription);

        // Act
        var result = await _subscriptionService.GetSubscriptionByIdAsync(subscriptionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedSubscription, result);
        _mockSubscriptionRepository.Verify(x => x.GetByIdAsync(subscriptionId), Times.Once);
    }

    [Fact]
    public async Task GetSubscriptionByIdAsync_WhenSubscriptionNotExists_ThrowsKeyNotFoundException()
    {
        // Arrange
        var subscriptionId = 1;
        _mockSubscriptionRepository.Setup(x => x.GetByIdAsync(subscriptionId))
            .ReturnsAsync((Subscription?)null);

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _subscriptionService.GetSubscriptionByIdAsync(subscriptionId));

        Assert.Contains($"Subscription with ID {subscriptionId} not found", exception.Message);
        _mockSubscriptionRepository.Verify(x => x.GetByIdAsync(subscriptionId), Times.Once);
    }

    [Fact]
    public async Task CreateDefaultSubscriptionAsync_WithFreeType_CreatesSubscriptionWithMaxDate()
    {
        // Arrange
        _mockSubscriptionRepository.Setup(x => x.AddAsync(It.IsAny<Subscription>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _subscriptionService.CreateSubscriptionAsync(SubscriptionType.Free);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(SubscriptionType.Free, result.Type);
        Assert.True(Math.Abs((DateTime.UtcNow - result.StartDate).TotalSeconds) < 5);
        Assert.Equal(SubscriptionConstants.MAX_SUBSCRIPTION_DATE, result.EndDate);
        _mockSubscriptionRepository.Verify(x => x.AddAsync(It.IsAny<Subscription>()), Times.Once);
    }

    [Fact]
    public async Task DeleteSubscriptionAsync_WhenCalled_CallsRepository()
    {
        // Arrange
        var subscriptionId = 1;
        _mockSubscriptionRepository.Setup(x => x.DeleteAsync(subscriptionId))
            .Returns(Task.CompletedTask);

        // Act
        await _subscriptionService.DeleteSubscriptionAsync(subscriptionId);

        // Assert
        _mockSubscriptionRepository.Verify(x => x.DeleteAsync(subscriptionId), Times.Once);
    }
    
    [Fact]
    public async Task GetAllSubscriptionsAsync_ReturnsAllSubscriptions()
    {
        // Arrange
        var subscriptions = new List<Subscription>
        {
            new Subscription(SubscriptionType.Free, DateTime.UtcNow, SubscriptionConstants.MAX_SUBSCRIPTION_DATE),
            new Subscription(SubscriptionType.Trial, DateTime.UtcNow,
                DateTime.UtcNow.AddMonths(SubscriptionConstants.TRIAL_DURATION_MONTHS)),
            new Subscription(SubscriptionType.Super, DateTime.UtcNow,
                DateTime.UtcNow.AddMonths(SubscriptionConstants.SUPER_DURATION_MONTHS))
        };

        _mockSubscriptionRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(subscriptions);

        // Act
        var result = await _subscriptionService.GetAllSubscriptionsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        _mockSubscriptionRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }
}