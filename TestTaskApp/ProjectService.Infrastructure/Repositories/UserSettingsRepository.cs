using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using ProjectService.Application.Repositories;
using ProjectService.Domain.Entities;
using ProjectService.Infrastructure.Persistence;

namespace ProjectService.Infrastructure.Repositories;

public class UserSettingsRepository : IUserSettingsRepository
{
    private readonly IMongoCollection<UserSettings> _userSettings;
    private readonly ILogger<UserSettingsRepository> _logger;
    
    public UserSettingsRepository(MongoDbContext context, ILogger<UserSettingsRepository> logger)
    {
        _userSettings = context.UserSettings;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<UserSettings> GetByIdAsync(ObjectId id)
    {
        try
        {
            return await _userSettings.Find(us => us.Id == id).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user settings by ID: {Id}", id);
            throw;
        }
    }

    public async Task<UserSettings> GetByUserIdAsync(int userId)
    {
        try
        {
            return await _userSettings.Find(us => us.UserId == userId).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user settings by user ID: {UserId}", userId);
            throw;
        }
    }

    public async Task<IEnumerable<UserSettings>> GetAllAsync()
    {
        try
        {
            return await _userSettings.Find(_ => true).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all user settings");
            throw;
        }
    }

    public async Task AddAsync(UserSettings userSettings)
    {
        try
        {
            await _userSettings.InsertOneAsync(userSettings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user settings for user ID: {UserId}", userSettings.UserId);
            throw;
        }
    }

    public async Task UpdateAsync(UserSettings userSettings)
    {
        try
        {
            var result = await _userSettings.ReplaceOneAsync(us => us.Id == userSettings.Id, userSettings);
            if (result.MatchedCount == 0)
            {
                throw new KeyNotFoundException($"User settings with ID {userSettings.Id} not found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user settings with ID: {Id}", userSettings.Id);
            throw;
        }
    }

    public async Task DeleteAsync(ObjectId id)
    {
        try
        {
            var result = await _userSettings.DeleteOneAsync(us => us.Id == id);
            if (result.DeletedCount == 0)
            {
                throw new KeyNotFoundException($"User settings with ID {id} not found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user settings with ID: {Id}", id);
            throw;
        }
    }

    public async Task DeleteByUserIdAsync(int userId)
    {
        try
        {
            var result = await _userSettings.DeleteOneAsync(us => us.UserId == userId);
            if (result.DeletedCount == 0)
            {
                throw new KeyNotFoundException($"User settings for user ID {userId} not found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user settings for user ID: {UserId}", userId);
            throw;
        }
    }
}