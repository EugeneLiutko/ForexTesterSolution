using UserService.Domain.Entities;

namespace UserService.Application.Repositories;

public interface ISubscriptionRepository
{
    Task<IEnumerable<Subscription>> GetAllAsync();
    Task<Subscription> GetByIdAsync(int id);
    Task AddAsync(Subscription subscription);
    Task UpdateAsync(Subscription subscription);
    Task DeleteAsync(int id);
}