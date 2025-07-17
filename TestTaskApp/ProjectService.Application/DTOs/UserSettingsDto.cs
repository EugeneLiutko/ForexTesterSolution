namespace ProjectService.Application.DTOs;

public record UserSettingsDto(
    string Id,
    int UserId,
    string Language,
    string Theme
);