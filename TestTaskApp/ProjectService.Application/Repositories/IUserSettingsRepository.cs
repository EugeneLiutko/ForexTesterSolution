using MongoDB.Bson;
using ProjectService.Domain.Entities;

namespace ProjectService.Application.Repositories;

public interface IUserSettingsRepository
{
    Task<UserSettings> GetByIdAsync(ObjectId id);
    Task<UserSettings> GetByUserIdAsync(int userId);
    Task<IEnumerable<UserSettings>> GetAllAsync();
    Task AddAsync(UserSettings userSettings);
    Task UpdateAsync(UserSettings userSettings);
    Task DeleteAsync(ObjectId id);
    Task DeleteByUserIdAsync(int userId);
}