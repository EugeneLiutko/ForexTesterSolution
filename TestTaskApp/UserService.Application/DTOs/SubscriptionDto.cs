namespace UserService.Application.DTOs;

public record SubscriptionDto(
    int Id,
    string Type,
    DateTime StartDate,
    DateTime EndDate
);