using MongoDB.Driver;
using ProjectService.Domain.Entities;
using ProjectService.Domain.Enums;

namespace ProjectService.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(MongoDbContext mongoContext)
    {
        try
        {
            Console.WriteLine("Starting MongoDB database initialization...");
            
            await mongoContext.Projects.Database.RunCommandAsync((Command<MongoDB.Bson.BsonDocument>)"{ping:1}");
            Console.WriteLine("MongoDB connection successful.");

            await EnsureDataExistsAsync(mongoContext);
            
            Console.WriteLine("MongoDB database initialization completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"MongoDB database initialization failed: {ex.Message}");
            throw;
        }
    }

    private static async Task EnsureDataExistsAsync(MongoDbContext context)
    {
        await EnsureProjectsAsync(context);
        await EnsureUserSettingsAsync(context);
    }

    private static async Task EnsureProjectsAsync(MongoDbContext context)
    {
        try
        {
            var projectsCount = await context.Projects.CountDocumentsAsync(FilterDefinition<Project>.Empty);

            if (projectsCount == 0)
            {
                Console.WriteLine("No projects found. Creating projects...");

                var projects = new List<Project>
                {
                    new()
                    {
                        UserId = 3,
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
                        UserId = 3,
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
                        UserId = 3,
                        Name = "my super project 3",
                        Charts = new List<Chart>()
                    },
                    new()
                    {
                        UserId = 2,
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
                    },
                    new()
                    {
                        UserId = 2,
                        Name = "project 2",
                        Charts = new List<Chart>
                        {
                            new()
                            {
                                Symbol = Symbol.USDJPY,
                                Timeframe = Timeframe.H1,
                                Indicators = new List<Indicator>
                                {
                                    new() { Name = IndicatorName.MA, Parameters = "a=1;b=2;c=3" }
                                }
                            }
                        }
                    },
                    new()
                    {
                        UserId = 1,
                        Name = "project 3",
                        Charts = new List<Chart>
                        {
                            new()
                            {
                                Symbol = Symbol.EURUSD,
                                Timeframe = Timeframe.M5,
                                Indicators = new List<Indicator>
                                {
                                    new() { Name = IndicatorName.RSI, Parameters = "a=1;b=2;c=3" },
                                    new() { Name = IndicatorName.MA, Parameters = "a=1;b=2;c=3" }
                                }
                            }
                        }
                    }
                };

                await context.Projects.InsertManyAsync(projects);
                Console.WriteLine($"Created {projects.Count} projects.");
            }
            else
            {
                Console.WriteLine($"Found {projectsCount} projects. Skipping creation.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating projects: {ex.Message}");
            throw;
        }
    }

    private static async Task EnsureUserSettingsAsync(MongoDbContext context)
    {
        try
        {
            var userSettingsCount = await context.UserSettings.CountDocumentsAsync(FilterDefinition<UserSettings>.Empty);

            if (userSettingsCount == 0)
            {
                Console.WriteLine("No user settings found. Creating user settings...");

                var userSettings = new List<UserSettings>
                {
                    new()
                    {
                        UserId = 1,
                        Language = Language.English,
                        Theme = Theme.dark
                    },
                    new()
                    {
                        UserId = 2,
                        Language = Language.Spanish,
                        Theme = Theme.light
                    },
                    new()
                    {
                        UserId = 3,
                        Language = Language.English,
                        Theme = Theme.dark
                    }
                };

                await context.UserSettings.InsertManyAsync(userSettings);
                Console.WriteLine($"Created {userSettings.Count} user settings.");
            }
            else
            {
                Console.WriteLine($"Found {userSettingsCount} user settings. Skipping creation.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating user settings: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Швидка перевірка чи MongoDB ініціалізована
    /// </summary>
    public static async Task<bool> IsDatabaseInitializedAsync(MongoDbContext context)
    {
        try
        {
            var hasProjects = await context.Projects.CountDocumentsAsync(FilterDefinition<Project>.Empty) > 0;
            var hasUserSettings = await context.UserSettings.CountDocumentsAsync(FilterDefinition<UserSettings>.Empty) > 0;
            return hasProjects && hasUserSettings;
        }
        catch
        {
            return false;
        }
    }

    public static async Task<string> GetDatabaseStatusAsync(MongoDbContext context)
    {
        try
        {
            await context.Projects.Database.RunCommandAsync((Command<MongoDB.Bson.BsonDocument>)"{ping:1}");
            
            var projectsCount = await context.Projects.CountDocumentsAsync(FilterDefinition<Project>.Empty);
            var userSettingsCount = await context.UserSettings.CountDocumentsAsync(FilterDefinition<UserSettings>.Empty);

            var collections = await context.Projects.Database.ListCollectionNames().ToListAsync();
            var collectionsStr = string.Join(", ", collections);

            return $"Connected: true, Collections: [{collectionsStr}], Projects: {projectsCount}, UserSettings: {userSettingsCount}";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    /// <summary>
    /// Очищає всі дані в MongoDB (для тестів)
    /// </summary>
    public static async Task ClearAllDataAsync(MongoDbContext context)
    {
        try
        {
            Console.WriteLine("Clearing all MongoDB data...");
            
            await context.Projects.DeleteManyAsync(FilterDefinition<Project>.Empty);
            await context.UserSettings.DeleteManyAsync(FilterDefinition<UserSettings>.Empty);
            
            Console.WriteLine("All MongoDB data cleared.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error clearing MongoDB data: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Пересоздає тестові дані (для development)
    /// </summary>
    public static async Task RecreateTestDataAsync(MongoDbContext context)
    {
        try
        {
            Console.WriteLine("Recreating test data...");
            
            await ClearAllDataAsync(context);
            
            await EnsureDataExistsAsync(context);
            
            Console.WriteLine("Test data recreated successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error recreating test data: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Отримує статистику по проектах за типами користувачів
    /// </summary>
    public static async Task<Dictionary<int, int>> GetProjectsStatsByUserAsync(MongoDbContext context)
    {
        try
        {
            var projectsStats = new Dictionary<int, int>();
            
            var projects = await context.Projects.Find(FilterDefinition<Project>.Empty).ToListAsync();
            
            foreach (var project in projects)
            {
                if (projectsStats.ContainsKey(project.UserId))
                    projectsStats[project.UserId]++;
                else
                    projectsStats[project.UserId] = 1;
            }
            
            return projectsStats;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting projects stats: {ex.Message}");
            return new Dictionary<int, int>();
        }
    }
}