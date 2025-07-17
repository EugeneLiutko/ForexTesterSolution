using UserService.Domain.Entities;

namespace UserService.Tests;

public class SubscriptionTests
{
    [Theory]
    [InlineData(SubscriptionType.Trial, SubscriptionConstants.TRIAL_DURATION_MONTHS)]
    [InlineData(SubscriptionType.Super, SubscriptionConstants.SUPER_DURATION_MONTHS)]
    public void CreateWithDuration_WithTrialAndSuper_CreatesCorrectDuration(SubscriptionType type, int expectedMonths)
    {
        // Arrange
        var startDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        var subscription = Subscription.CreateWithDuration(type, startDate);

        // Assert
        Assert.NotNull(subscription);
        Assert.Equal(type, subscription.Type);
        Assert.Equal(startDate, subscription.StartDate);
        Assert.Equal(startDate.AddMonths(expectedMonths), subscription.EndDate);
    }

    [Fact]
    public void CreateWithDuration_WithFreeType_CreatesMaxDate()
    {
        // Arrange
        var startDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        var subscription = Subscription.CreateWithDuration(SubscriptionType.Free, startDate);

        // Assert
        Assert.NotNull(subscription);
        Assert.Equal(SubscriptionType.Free, subscription.Type);
        Assert.Equal(startDate, subscription.StartDate);
        Assert.Equal(SubscriptionConstants.MAX_SUBSCRIPTION_DATE, subscription.EndDate);
    }

    [Fact]
    public void CreateWithDuration_WithoutStartDate_UsesCurrentTime()
    {
        // Act
        var subscription = Subscription.CreateWithDuration(SubscriptionType.Trial);

        // Assert
        Assert.NotNull(subscription);
        Assert.Equal(SubscriptionType.Trial, subscription.Type);
        Assert.True(Math.Abs((DateTime.UtcNow - subscription.StartDate).TotalSeconds) < 5);
        Assert.True(Math.Abs((DateTime.UtcNow.AddMonths(SubscriptionConstants.TRIAL_DURATION_MONTHS) - subscription.EndDate).TotalSeconds) < 5);
    }

    [Fact]
    public void CreateWithDuration_WithFreeType_AlwaysUsesMaxDate()
    {
        // Arrange
        var startDate1 = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var startDate2 = new DateTime(2025, 6, 15, 10, 30, 0, DateTimeKind.Utc);

        // Act
        var subscription1 = Subscription.CreateWithDuration(SubscriptionType.Free, startDate1);
        var subscription2 = Subscription.CreateWithDuration(SubscriptionType.Free, startDate2);

        // Assert
        Assert.Equal(SubscriptionConstants.MAX_SUBSCRIPTION_DATE, subscription1.EndDate);
        Assert.Equal(SubscriptionConstants.MAX_SUBSCRIPTION_DATE, subscription2.EndDate);
        // Обидві підписки мають однакову кінцеву дату незалежно від старту
        Assert.Equal(subscription1.EndDate, subscription2.EndDate);
    }

    [Fact]
    public void IsActive_WhenCurrentTimeWithinRange_ReturnsTrue()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var subscription = new Subscription(SubscriptionType.Trial, now.AddDays(-1), now.AddDays(1));

        // Act
        var isActive = subscription.IsActive();

        // Assert
        Assert.True(isActive);
    }

    [Fact]
    public void IsActive_WhenCurrentTimeOutsideRange_ReturnsFalse()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var subscription = new Subscription(SubscriptionType.Trial, now.AddDays(-10), now.AddDays(-1));

        // Act
        var isActive = subscription.IsActive();

        // Assert
        Assert.False(isActive);
    }

    [Fact]
    public void IsActive_WithMaxSubscriptionDate_ReturnsTrue()
    {
        // Arrange
        var subscription = new Subscription(SubscriptionType.Free, DateTime.UtcNow.AddDays(-1), SubscriptionConstants.MAX_SUBSCRIPTION_DATE);

        // Act
        var isActive = subscription.IsActive();

        // Assert
        Assert.True(isActive); // Підписка активна до 2099 року
    }

    [Fact]
    public void CreateWithDuration_WithInvalidType_ThrowsArgumentException()
    {
        // Arrange
        var invalidType = (SubscriptionType)999;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => Subscription.CreateWithDuration(invalidType));
        
        Assert.Contains("Unknown subscription type", exception.Message);
    }

    [Fact]
    public void UpdateDetails_UpdatesSubscriptionProperties()
    {
        // Arrange
        var subscription = new Subscription(SubscriptionType.Free, DateTime.UtcNow, SubscriptionConstants.MAX_SUBSCRIPTION_DATE);
        var newType = SubscriptionType.Super;
        var newEndDate = DateTime.UtcNow.AddMonths(SubscriptionConstants.SUPER_DURATION_MONTHS);

        // Act
        subscription.UpdateDetails(newType, newEndDate);

        // Assert
        Assert.Equal(newType, subscription.Type);
        Assert.Equal(newEndDate, subscription.EndDate);
    }
}