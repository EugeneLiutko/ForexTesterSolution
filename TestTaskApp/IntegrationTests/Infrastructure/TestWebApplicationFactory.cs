using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.MongoDb;
using Testcontainers.PostgreSql;

namespace IntegrationTests.Infrastructure;

public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime where TProgram : class
{
    private readonly PostgreSqlContainer _postgresContainer;
    private readonly MongoDbContainer _mongoContainer;

    public TestWebApplicationFactory()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpass")
            .WithPortBinding(5433, true)
            .Build();

        _mongoContainer = new MongoDbBuilder()
            .WithImage("mongo:7.0")
            .WithUsername("testuser")
            .WithPassword("testpass")
            .WithPortBinding(27018, true)
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.Sources.Clear();
            
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                // PostgreSQL для UserService
                ["ConnectionStrings:DefaultConnection"] = _postgresContainer.GetConnectionString(),
                
                // MongoDB для ProjectService
                ["MongoSettings:ConnectionString"] = _mongoContainer.GetConnectionString(),
                ["MongoSettings:DatabaseName"] = "testprojectsdb",
                
                // Міжсервісна комунікація
                ["UserService:BaseUrl"] = "http://localhost:5001",
                
                // Logging
                ["Logging:LogLevel:Default"] = "Information",
                ["Logging:LogLevel:Microsoft.AspNetCore"] = "Warning"
            });
        });

        builder.UseEnvironment("Testing");
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        await _mongoContainer.StartAsync();
        
        // Почекати для повної ініціалізації контейнерів
        await Task.Delay(3000);
    }

    public new async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
        await _mongoContainer.DisposeAsync();
        await base.DisposeAsync();
    }

    public string GetPostgreSqlConnectionString() => _postgresContainer.GetConnectionString();
    public string GetMongoConnectionString() => _mongoContainer.GetConnectionString();
}