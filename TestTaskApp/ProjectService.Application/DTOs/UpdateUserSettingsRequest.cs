namespace ProjectService.Application.DTOs;

public record UpdateUserSettingsRequest(
    string Language,
    string Theme
);