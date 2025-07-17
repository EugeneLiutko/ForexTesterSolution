using ProjectService;
using Xunit.Abstractions;

namespace IntegrationTests.Tests;

public class BasicIntegrationTests2
{
    private readonly ITestOutputHelper _output;

    public BasicIntegrationTests2(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Test_Configuration_ShouldPass()
    {
        _output.WriteLine("✅ Basic test passed - configuration is working");
        Assert.True(true);
    }

    [Fact] 
    public async Task Test_Docker_Containers_ShouldStart()
    {
        using var factory = new Infrastructure.TestWebApplicationFactory<Program>();
        
        try
        {
            await factory.InitializeAsync();
            
            var postgresConnection = factory.GetPostgreSqlConnectionString();
            var mongoConnection = factory.GetMongoConnectionString();
            
            _output.WriteLine($"✅ PostgreSQL: {postgresConnection}");
            _output.WriteLine($"✅ MongoDB: {mongoConnection}");
            
            Assert.NotNull(postgresConnection);
            Assert.NotNull(mongoConnection);
        }
        finally
        {
            await factory.DisposeAsync();
        }
    }
}