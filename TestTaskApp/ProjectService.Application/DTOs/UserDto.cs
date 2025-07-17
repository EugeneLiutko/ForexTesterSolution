namespace ProjectService.Application.DTOs;

public record UserDto(
    int Id,
    string Name,
    string Email,
    int SubscriptionId
);