using Microsoft.Extensions.Logging;
using UserService.Application.DTOs;
using UserService.Application.Extensions;
using UserService.Application.Repositories;
using UserService.Domain.Entities;

namespace UserService.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ISubscriptionService _subscriptionService;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ISubscriptionService subscriptionService,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _subscriptionService = subscriptionService ?? throw new ArgumentNullException(nameof(subscriptionService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException($"User with ID {id} not found");
    }

    public async Task CreateUserAsync(string name, string email)
    {
        var subscription = await _subscriptionService.CreateSubscriptionAsync(SubscriptionType.Free);

        var user = new User(name, email, subscription.Id);
        await _userRepository.AddAsync(user);
    }

    public async Task UpdateUserAsync(int id, string name, string email)
    {
        var user = await _userRepository.GetByIdAsync(id) ??
                   throw new KeyNotFoundException($"User with ID {id} not found");
        user.UpdateDetails(name, email);
        await _userRepository.UpdateAsync(user);
    }

    public async Task DeleteUserAsync(int id)
    {
        await _userRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<UserDto>> GetUsersBySubscriptionTypeAsync(SubscriptionType subscriptionType)
    {
        _logger.LogInformation("Getting users with subscription type: {SubscriptionType}", subscriptionType);

        var users = await _userRepository.GetUsersBySubscriptionTypeAsync(subscriptionType);

        _logger.LogInformation("Found {UserCount} users with subscription type: {SubscriptionType}",
            users.Count(), subscriptionType);

        return users.Select(u => u.ToDto());
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(u => u.ToDto());
    }
}