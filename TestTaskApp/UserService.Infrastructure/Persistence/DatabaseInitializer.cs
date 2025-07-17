using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Persistence;
public static class DatabaseInitializer
{
    public static async Task InitializeAsync(UserDbContext context)
    {
        try
        {
            Console.WriteLine("Starting database initialization...");
            
            var created = await context.Database.EnsureCreatedAsync();
            if (created)
            {
                Console.WriteLine("Database and tables created successfully.");
            }
            else
            {
                Console.WriteLine("Database already exists.");
            }

            await EnsureDataExistsAsync(context);
            
            Console.WriteLine("Database initialization completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database initialization failed: {ex.Message}");
            throw;
        }
    }

    private static async Task ApplyMigrationsAsync(UserDbContext context)
    {
        try
        {
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            
            if (pendingMigrations.Any())
            {
                Console.WriteLine($"Applying {pendingMigrations.Count()} pending migrations...");
                foreach (var migration in pendingMigrations)
                {
                    Console.WriteLine($"- {migration}");
                }
                
                await context.Database.MigrateAsync();
                Console.WriteLine("Migrations applied successfully.");
            }
            else
            {
                Console.WriteLine("No pending migrations found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Migration failed: {ex.Message}");
            throw;
        }
    }
    
    private static async Task EnsureDataExistsAsync(UserDbContext context)
    {
        await EnsureSubscriptionsAsync(context);
        await EnsureUsersAsync(context);
    }

    private static async Task EnsureSubscriptionsAsync(UserDbContext context)
    {
        try
        {
            var subscriptionsCount = await context.Subscriptions.CountAsync();

            if (subscriptionsCount == 0)
            {
                Console.WriteLine("No subscriptions found. Creating subscriptions...");

                var subscriptions = new List<Subscription>
                {
                    new Subscription(SubscriptionType.Free, 
                        new DateTime(2022, 5, 17, 15, 28, 19, DateTimeKind.Utc), 
                        new DateTime(2099, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
                    new Subscription(SubscriptionType.Super, 
                        new DateTime(2022, 5, 18, 15, 28, 19, DateTimeKind.Utc), 
                        new DateTime(2022, 8, 18, 15, 28, 19, DateTimeKind.Utc)),
                    new Subscription(SubscriptionType.Trial, 
                        new DateTime(2022, 5, 19, 15, 28, 19, DateTimeKind.Utc), 
                        new DateTime(2022, 6, 19, 15, 28, 19, DateTimeKind.Utc)),
                    new Subscription(SubscriptionType.Free, 
                        new DateTime(2022, 5, 20, 15, 28, 19, DateTimeKind.Utc), 
                        new DateTime(2099, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
                    new Subscription(SubscriptionType.Trial, 
                        new DateTime(2022, 5, 21, 15, 28, 19, DateTimeKind.Utc), 
                        new DateTime(2022, 6, 21, 15, 28, 19, DateTimeKind.Utc)),
                    new Subscription(SubscriptionType.Super, 
                        new DateTime(2022, 5, 22, 15, 28, 19, DateTimeKind.Utc), 
                        new DateTime(2023, 5, 22, 15, 28, 19, DateTimeKind.Utc)),
                    new Subscription(SubscriptionType.Super, 
                        new DateTime(2022, 5, 23, 15, 28, 19, DateTimeKind.Utc), 
                        new DateTime(2023, 5, 23, 15, 28, 19, DateTimeKind.Utc))
                };


                await context.Subscriptions.AddRangeAsync(subscriptions);
                await context.SaveChangesAsync();

                Console.WriteLine($"Created {subscriptions.Count} subscriptions.");
            }
            else
            {
                Console.WriteLine($"Found {subscriptionsCount} subscriptions. Skipping creation.");
            }
        }
        catch (Exception ex) when (ex.Message.Contains("does not exist"))
        {
            Console.WriteLine("Subscriptions table does not exist. Applying migrations...");
            await context.Database.MigrateAsync();

            await EnsureSubscriptionsAsync(context);
        }
    }

    private static async Task EnsureUsersAsync(UserDbContext context)
    {
        try
        {
            var usersCount = await context.Users.CountAsync();

            if (usersCount == 0)
            {
                Console.WriteLine("No users found. Creating users...");

                var subscriptionsCount = await context.Subscriptions.CountAsync();
                if (subscriptionsCount < 7)
                {
                    throw new InvalidOperationException(
                        $"Not enough subscriptions found. Expected at least 7, found {subscriptionsCount}.");
                }

                var users = new List<User>
                {
                    new User("John Doe", "John@example.com", 2),
                    new User("Mark Shimko", "Mark@example.com", 5),
                    new User("Taras Ovruch", "Taras@example.com", 6)
                };

                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();

                Console.WriteLine($"Created {users.Count} users.");
            }
            else
            {
                Console.WriteLine($"Found {usersCount} users. Skipping creation.");
            }
        }
        catch (Exception ex) when (ex.Message.Contains("does not exist"))
        {
            Console.WriteLine("Users table does not exist. Applying migrations...");
            await context.Database.MigrateAsync();

            await EnsureUsersAsync(context);
        }
    }

    /// <summary>
    /// Швидка перевірка чи БД ініціалізована
    /// </summary>
    public static async Task<bool> IsDatabaseInitializedAsync(UserDbContext context)
    {
        try
        {
            var hasUsers = await context.Users.AnyAsync();
            var hasSubscriptions = await context.Subscriptions.AnyAsync();
            return hasUsers && hasSubscriptions;
        }
        catch
        {
            return false;
        }
    }
    
    public static async Task<string> GetDatabaseStatusAsync(UserDbContext context)
    {
        try
        {
            var canConnect = await context.Database.CanConnectAsync();
            if (!canConnect) return "Cannot connect to database";

            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();
            
            var usersCount = await context.Users.CountAsync();
            var subscriptionsCount = await context.Subscriptions.CountAsync();

            return $"Connected: {canConnect}, Applied migrations: {appliedMigrations.Count()}, Pending migrations: {pendingMigrations.Count()}, Users: {usersCount}, Subscriptions: {subscriptionsCount}";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
}