using UserService.Application.DTOs;
using UserService.Domain.Entities;

namespace UserService.Application.Extensions;

public static class MappingExtensions
{
    // User mappings
    public static UserDto ToDto(this User user)
    {
        return new UserDto(
            user.Id,
            user.Name,
            user.Email,
            user.SubscriptionId,
            user.Subscription?.ToDto()
        );
    }

    public static UserDto ToDtoWithoutSubscription(this User user)
    {
        return new UserDto(
            user.Id,
            user.Name,
            user.Email,
            user.SubscriptionId,
            null
        );
    }

    // Subscription mappings
    public static SubscriptionDto ToDto(this Subscription subscription)
    {
        return new SubscriptionDto(
            subscription.Id,
            subscription.Type.ToString(),
            subscription.StartDate,
            subscription.EndDate
        );
    }
}