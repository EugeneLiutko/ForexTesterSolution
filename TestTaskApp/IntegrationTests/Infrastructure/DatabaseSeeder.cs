using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using UserService.Infrastructure.Persistence;
using UserService.Domain.Entities;
using ProjectService.Domain.Entities;
using ProjectService.Domain.Enums;
using SubscriptionType = UserService.Domain.Entities.SubscriptionType;

namespace IntegrationTests.Infrastructure;

public static class DatabaseSeeder
{
    public static async Task SeedUserServiceDatabase(string connectionString)
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        using var context = new UserDbContext(options);
        
        try
        {
            await context.Database.EnsureCreatedAsync();
            
            context.Users.RemoveRange(context.Users);
            context.Subscriptions.RemoveRange(context.Subscriptions);
            await context.SaveChangesAsync();

            var subscriptions = new List<Subscription>
            {
                Subscription.CreateWithDuration(SubscriptionType.Free),
                Subscription.CreateWithDuration(SubscriptionType.Super, DateTime.UtcNow.AddDays(-10)),
                Subscription.CreateWithDuration(SubscriptionType.Trial, DateTime.UtcNow.AddDays(-5)),
                Subscription.CreateWithDuration(SubscriptionType.Free, DateTime.UtcNow.AddDays(-60)),
                Subscription.CreateWithDuration(SubscriptionType.Trial, DateTime.UtcNow.AddDays(-3)),
                Subscription.CreateWithDuration(SubscriptionType.Super, DateTime.UtcNow.AddDays(-20))
            };

            context.Subscriptions.AddRange(subscriptions);
            await context.SaveChangesAsync();

            var savedSubscriptions = await context.Subscriptions.ToListAsync();

            var users = new List<User>
            {
                new("John Doe", "john@example.com", savedSubscriptions[1].Id),    // Super subscription
                new("Mark Shimko", "mark@example.com", savedSubscriptions[4].Id), // Trial subscription  
                new("Taras Ovruch", "taras@example.com", savedSubscriptions[5].Id), // Super subscription
                new("Alice Smith", "alice@example.com", savedSubscriptions[0].Id), // Free subscription
                new("Bob Johnson", "bob@example.com", savedSubscriptions[3].Id)  // Free subscription
            };

            context.Users.AddRange(users);
            await context.SaveChangesAsync();

            Console.WriteLine($"✅ UserService database seeded with {users.Count} users and {subscriptions.Count} subscriptions");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error seeding UserService database: {ex.Message}");
            throw;
        }
    }

    public static async Task SeedProjectServiceDatabase(string connectionString, string databaseName)
    {
        try
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            
            await database.DropCollectionAsync("projects");
            await database.DropCollectionAsync("user.settings");

            var projectsCollection = database.GetCollection<Project>("projects");
            var userSettingsCollection = database.GetCollection<UserSettings>("user.settings");

            var projects = new List<Project>
            {
                new()
                {
                    UserId = 1, // John (Super subscription)
                    Name = "my super project 1",
                    Charts = new List<Chart>
                    {
                        new()
                        {
                            Symbol = Symbol.EURUSD,
                            Timeframe = Timeframe.M5,
                            Indicators = new List<Indicator>()
                        },
                        new()
                        {
                            Symbol = Symbol.USDJPY,
                            Timeframe = Timeframe.H1,
                            Indicators = new List<Indicator>
                            {
                                new() { Name = IndicatorName.BB, Parameters = "a=1;b=2;c=3" },
                                new() { Name = IndicatorName.MA, Parameters = "a=1;b=2;c=3" }
                            }
                        }
                    }
                },
                new()
                {
                    UserId = 3, // Taras (Super subscription)
                    Name = "my super project 2",
                    Charts = new List<Chart>
                    {
                        new()
                        {
                            Symbol = Symbol.EURUSD,
                            Timeframe = Timeframe.M5,
                            Indicators = new List<Indicator>
                            {
                                new() { Name = IndicatorName.MA, Parameters = "a=1;b=2;c=3" }
                            }
                        }
                    }
                },
                new()
                {
                    UserId = 2, // Mark (Trial subscription)
                    Name = "project 1",
                    Charts = new List<Chart>
                    {
                        new()
                        {
                            Symbol = Symbol.EURUSD,
                            Timeframe = Timeframe.H1,
                            Indicators = new List<Indicator>
                            {
                                new() { Name = IndicatorName.RSI, Parameters = "a=1;b=2;c=3" }
                            }
                        }
                    }
                }
            };

            await projectsCollection.InsertManyAsync(projects);

            var userSettings = new List<UserSettings>
            {
                new() { UserId = 1, Language = Language.English, Theme = Theme.dark },
                new() { UserId = 2, Language = Language.Spanish, Theme = Theme.light },
                new() { UserId = 3, Language = Language.English, Theme = Theme.dark }
            };

            await userSettingsCollection.InsertManyAsync(userSettings);

            Console.WriteLine($"✅ ProjectService database seeded with {projects.Count} projects and {userSettings.Count} user settings");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error seeding ProjectService database: {ex.Message}");
            throw;
        }
    }
}
