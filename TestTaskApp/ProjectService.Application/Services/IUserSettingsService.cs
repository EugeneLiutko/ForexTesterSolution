using ProjectService.Application.DTOs;

namespace ProjectService.Application.Services;

public interface IUserSettingsService
{
    Task<UserSettingsDto> GetUserSettingsByIdAsync(string id);
    Task<UserSettingsDto> GetUserSettingsByUserIdAsync(int userId);
    Task<IEnumerable<UserSettingsDto>> GetAllUserSettingsAsync();
    Task<UserSettingsDto> CreateUserSettingsAsync(CreateUserSettingsRequest request);
    Task<UserSettingsDto> UpdateUserSettingsAsync(string id, UpdateUserSettingsRequest request);
    Task DeleteUserSettingsAsync(string id);
    Task DeleteUserSettingsByUserIdAsync(int userId);
}