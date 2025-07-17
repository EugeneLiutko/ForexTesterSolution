using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.Application.Repositories;
using UserService.Domain.Entities;
using UserService.Infrastructure.Persistence;

namespace UserService.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(UserDbContext context,  ILogger<UserRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<User> GetByIdAsync(int id)
    {
        return await _context.Users.Include(u => u.Subscription).FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
    
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        try
        {
            _logger.LogDebug("Getting all users");
            
            var users = await _context.Users.ToListAsync();
            
            _logger.LogDebug("Found {UsersCount} total users", users.Count);
            
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users");
            throw;
        }
    }

    public async Task<IEnumerable<User>> GetUsersBySubscriptionTypeAsync(SubscriptionType subscriptionType)
    {
        try
        {
            _logger.LogDebug("Getting users with subscription type: {SubscriptionType}", subscriptionType);
            
            var users = await _context.Users
                .Include(u => u.Subscription)
                .Where(u => u.Subscription.Type == subscriptionType)
                .ToListAsync();
            
            _logger.LogDebug("Found {UserCount} users with subscription type: {SubscriptionType}", 
                users.Count, subscriptionType);
            
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users by subscription type: {SubscriptionType}", subscriptionType);
            throw;
        }
    }
}