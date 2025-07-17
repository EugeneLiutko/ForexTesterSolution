namespace ProjectService.Application.DTOs;

public record CreateUserSettingsRequest(
    int UserId,
    string Language,
    string Theme
);