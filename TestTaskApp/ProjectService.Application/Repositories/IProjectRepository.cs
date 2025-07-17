using MongoDB.Bson;
using ProjectService.Domain.Entities;

namespace ProjectService.Application.Repositories;

public interface IProjectRepository
{
    Task<Project> GetByIdAsync(ObjectId id);
    Task<IEnumerable<Project>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Project>> GetAllAsync();
    Task AddAsync(Project project);
    Task UpdateAsync(Project project);
    Task DeleteAsync(ObjectId id);
    Task<IEnumerable<Project>> GetProjectsByUserIdsAsync(IEnumerable<int> userIds);
}