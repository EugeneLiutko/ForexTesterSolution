using UserService.Domain.Entities;

namespace UserService.Application.Repositories;

public interface IUserRepository
{
    Task<User> GetByIdAsync(int id);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(int id);
    Task<IEnumerable<User>> GetAllAsync();
    Task<IEnumerable<User>> GetUsersBySubscriptionTypeAsync(SubscriptionType subscriptionType);
}