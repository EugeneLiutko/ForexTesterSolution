namespace UserService.Application.DTOs;

public record UserDto(
    int Id,
    string Name,
    string Email,
    int SubscriptionId,
    SubscriptionDto? Subscription = null
);