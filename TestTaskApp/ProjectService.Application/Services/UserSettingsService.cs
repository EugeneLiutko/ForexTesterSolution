using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using ProjectService.Application.DTOs;
using ProjectService.Application.Extensions;
using ProjectService.Application.Repositories;
using ProjectService.Domain.Enums;

namespace ProjectService.Application.Services;
public class UserSettingsService : IUserSettingsService
{
    private readonly IUserSettingsRepository _userSettingsRepository;
    private readonly ILogger<UserSettingsService> _logger;

    public UserSettingsService(
        IUserSettingsRepository userSettingsRepository,
        ILogger<UserSettingsService> logger)
    {
        _userSettingsRepository = userSettingsRepository ?? throw new ArgumentNullException(nameof(userSettingsRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<UserSettingsDto> GetUserSettingsByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
        {
            throw new ArgumentException("Invalid user settings ID format", nameof(id));
        }

        var userSettings = await _userSettingsRepository.GetByIdAsync(objectId);
        if (userSettings == null)
        {
            throw new KeyNotFoundException($"User settings with ID {id} not found");
        }

        return userSettings.ToDto();
    }

    public async Task<UserSettingsDto> GetUserSettingsByUserIdAsync(int userId)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("User ID must be positive", nameof(userId));
        }

        var userSettings = await _userSettingsRepository.GetByUserIdAsync(userId);
        if (userSettings == null)
        {
            throw new KeyNotFoundException($"User settings for user ID {userId} not found");
        }

        return userSettings.ToDto();
    }

    public async Task<IEnumerable<UserSettingsDto>> GetAllUserSettingsAsync()
    {
        var userSettings = await _userSettingsRepository.GetAllAsync();
        return userSettings.Select(us => us.ToDto());
    }

    public async Task<UserSettingsDto> CreateUserSettingsAsync(CreateUserSettingsRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (request.UserId <= 0)
        {
            throw new ArgumentException("User ID must be positive", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Language))
        {
            throw new ArgumentException("Language is required", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Theme))
        {
            throw new ArgumentException("Theme is required", nameof(request));
        }

        var existingSettings = await _userSettingsRepository.GetByUserIdAsync(request.UserId);
        if (existingSettings != null)
        {
            throw new InvalidOperationException($"User settings for user ID {request.UserId} already exist");
        }

        if (!Enum.TryParse<Language>(request.Language, true, out var language))
        {
            throw new ArgumentException("Invalid language. Valid values: English, Spanish", nameof(request));
        }

        if (!Enum.TryParse<Theme>(request.Theme, true, out var theme))
        {
            throw new ArgumentException("Invalid theme. Valid values: light, dark", nameof(request));
        }

        var userSettings = request.ToEntity();
        await _userSettingsRepository.AddAsync(userSettings);
        
        _logger.LogInformation("Created user settings for user ID: {UserId}", request.UserId);
        
        return userSettings.ToDto();
    }

    public async Task<UserSettingsDto> UpdateUserSettingsAsync(string id, UpdateUserSettingsRequest request)
    {
        if (!ObjectId.TryParse(id, out var objectId))
        {
            throw new ArgumentException("Invalid user settings ID format", nameof(id));
        }

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Language))
        {
            throw new ArgumentException("Language is required", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Theme))
        {
            throw new ArgumentException("Invalid theme. Valid values: light, dark", nameof(request));
        }

        var existingSettings = await _userSettingsRepository.GetByIdAsync(objectId);
        if (existingSettings == null)
        {
            throw new KeyNotFoundException($"User settings with ID {id} not found");
        }

        if (!Enum.TryParse<Language>(request.Language, true, out var language))
        {
            throw new ArgumentException("Invalid language. Valid values: English, Spanish", nameof(request));
        }

        if (!Enum.TryParse<Theme>(request.Theme, true, out var theme))
        {
            throw new ArgumentException("Invalid theme. Valid values: light, dark", nameof(request));
        }

        existingSettings.Language = language;
        existingSettings.Theme = theme;

        await _userSettingsRepository.UpdateAsync(existingSettings);
        
        _logger.LogInformation("Updated user settings with ID: {Id}", id);
        
        return existingSettings.ToDto();
    }

    public async Task DeleteUserSettingsAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
        {
            throw new ArgumentException("Invalid user settings ID format", nameof(id));
        }

        var existingSettings = await _userSettingsRepository.GetByIdAsync(objectId);
        if (existingSettings == null)
        {
            throw new KeyNotFoundException($"User settings with ID {id} not found");
        }

        await _userSettingsRepository.DeleteAsync(objectId);
        
        _logger.LogInformation("Deleted user settings with ID: {Id}", id);
    }

    public async Task DeleteUserSettingsByUserIdAsync(int userId)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("User ID must be positive", nameof(userId));
        }

        var existingSettings = await _userSettingsRepository.GetByUserIdAsync(userId);
        if (existingSettings == null)
        {
            throw new KeyNotFoundException($"User settings for user ID {userId} not found");
        }

        await _userSettingsRepository.DeleteByUserIdAsync(userId);
        
        _logger.LogInformation("Deleted user settings for user ID: {UserId}", userId);
    }
}
