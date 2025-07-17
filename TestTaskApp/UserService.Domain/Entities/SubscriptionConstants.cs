namespace UserService.Domain.Entities;

public static class SubscriptionConstants
{
    public const int TRIAL_DURATION_MONTHS = 1;
    public const int SUPER_DURATION_MONTHS = 3;
    public static readonly DateTime MAX_SUBSCRIPTION_DATE = new DateTime(2099, 1, 1, 0, 0, 0, DateTimeKind.Utc);
}