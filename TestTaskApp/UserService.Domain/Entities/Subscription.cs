using System.Text.Json.Serialization;

namespace UserService.Domain.Entities;

public class Subscription
{
    public int Id { get; private set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SubscriptionType Type { get; private set; } // "Free", "Trial", "Super"

    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    [JsonIgnore] public User? User { get; private set; } 

    private Subscription()
    {
    }

    public Subscription(SubscriptionType type, DateTime startDate, DateTime endDate)
    {
        Type = type;
        StartDate = startDate;
        EndDate = endDate;
    }

    public void UpdateDetails(SubscriptionType type, DateTime endDate)
    {
        Type = type;
        EndDate = endDate;
    }

    public static Subscription CreateWithDuration(SubscriptionType type, DateTime? startDate = null)
    {
        var start = startDate ?? DateTime.UtcNow;
        DateTime end;

        switch (type)
        {
            case SubscriptionType.Free:
                end = SubscriptionConstants.MAX_SUBSCRIPTION_DATE;
                break;
            
            case SubscriptionType.Trial:
                end = start.AddMonths(SubscriptionConstants.TRIAL_DURATION_MONTHS);
                break;
            
            case SubscriptionType.Super:
                end = start.AddMonths(SubscriptionConstants.SUPER_DURATION_MONTHS);
                break;
            
            default:
                throw new ArgumentException($"Unknown subscription type: {type}");
        }

        return new Subscription(type, start, end);
    }

    public bool IsActive()
    {
        var now = DateTime.UtcNow;
        return now >= StartDate && now <= EndDate;
    }

    public string GetTypeString()
    {
        return Type.ToString();
    }

    public bool IsAvailable()
    {
        return User == null;
    }
}