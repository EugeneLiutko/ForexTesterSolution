using UserService.Domain.Entities;

namespace UserService.Tests;

public class SubscriptionConstantsTests
{
    [Fact]
    public void SubscriptionConstants_HaveCorrectValues()
    {
        // Assert
        Assert.Equal(1, SubscriptionConstants.TRIAL_DURATION_MONTHS);
        Assert.Equal(3, SubscriptionConstants.SUPER_DURATION_MONTHS);
        Assert.Equal(new DateTime(2099, 1, 1, 0, 0, 0, DateTimeKind.Utc), SubscriptionConstants.MAX_SUBSCRIPTION_DATE);
    }

    [Fact]
    public void MaxSubscriptionDate_IsInFuture()
    {
        // Assert
        Assert.True(SubscriptionConstants.MAX_SUBSCRIPTION_DATE > DateTime.UtcNow);
    }
}