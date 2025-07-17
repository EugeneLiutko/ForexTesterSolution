using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProjectService.Domain.Entities;
using ProjectService.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;

namespace ProjectService.Infrastructure.Persistence;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;
    private readonly ILogger<MongoDbContext> _logger;

    public MongoDbContext(IOptions<MongoDbSettings> settings, ILogger<MongoDbContext> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var mongoSettings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));

        if (string.IsNullOrEmpty(mongoSettings.ConnectionString))
        {
            throw new InvalidOperationException("MongoDB connection string is not configured");
        }

        if (string.IsNullOrEmpty(mongoSettings.DatabaseName))
        {
            throw new InvalidOperationException("MongoDB database name is not configured");
        }

        try
        {
            _logger.LogInformation("Connecting to MongoDB with connection string: {ConnectionString}",
                mongoSettings.ConnectionString.Substring(0, Math.Min(50, mongoSettings.ConnectionString.Length)) +
                "...");

            var client = new MongoClient(mongoSettings.ConnectionString);
            _database = client.GetDatabase(mongoSettings.DatabaseName);

            _logger.LogInformation("Successfully connected to MongoDB database: {DatabaseName}",
                mongoSettings.DatabaseName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to MongoDB");
            throw;
        }
    }

    public IMongoCollection<Project> Projects
    {
        get
        {
            try
            {
                var collection = _database.GetCollection<Project>("projects");
                _logger.LogDebug("Retrieved Projects collection");
                return collection;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get Projects collection");
                throw;
            }
        }
    }

    public IMongoCollection<UserSettings> UserSettings
    {
        get
        {
            try
            {
                var collection = _database.GetCollection<UserSettings>("user.settings");
                _logger.LogDebug("Retrieved UserSettings collection");
                return collection;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get UserSettings collection");
                throw;
            }
        }
    }
}