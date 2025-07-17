using UserService.Application.DTOs;
using UserService.Domain.Entities;

namespace UserService.Application.Services;

public interface IUserService
{
    Task<User> GetUserByIdAsync(int id);
    Task CreateUserAsync(string name, string email);
    Task UpdateUserAsync(int id, string name, string email);
    Task DeleteUserAsync(int id);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<IEnumerable<UserDto>> GetUsersBySubscriptionTypeAsync(SubscriptionType subscriptionType);
}