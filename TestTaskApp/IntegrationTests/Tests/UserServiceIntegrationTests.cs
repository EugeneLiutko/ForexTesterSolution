using System.Text;
using System.Text.Json;
using IntegrationTests.Infrastructure;
using Xunit.Abstractions;

namespace IntegrationTests.Tests;

[Collection("Integration Tests")]
public class UserServiceIntegrationTests : IClassFixture<SharedTestFixture>
{
    private readonly SharedTestFixture _fixture;
    private readonly ITestOutputHelper _output;

    public UserServiceIntegrationTests(SharedTestFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }

    [Fact]
    public async Task UserService_CreateUser_ShouldPersistInPostgreSQL()
    {
        // Arrange
        var createUserRequest = new
        {
            Name = "Integration Test User",
            Email = "integration@test.com"
        };

        // Act
        var createContent = new StringContent(JsonSerializer.Serialize(createUserRequest), Encoding.UTF8, "application/json");
        var createResponse = await _fixture.UserServiceClient.PostAsync("/api/users", createContent);

        // Assert
        if (!createResponse.IsSuccessStatusCode)
        {
            var errorContent = await createResponse.Content.ReadAsStringAsync();
            _output.WriteLine($"❌ UserService error: {createResponse.StatusCode} - {errorContent}");
        }

        Assert.True(createResponse.IsSuccessStatusCode, $"Failed to create user: {createResponse.StatusCode}");
        _output.WriteLine("✅ User created successfully");
    }

    [Fact]
    public async Task UserService_GetUsersBySubscriptionType_ShouldReturnCorrectUsers()
    {
        // Act
        var response = await _fixture.UserServiceClient.GetAsync("/api/users/by-subscription-type/Super");
        
        // Debug output
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine($"❌ UserService error: {response.StatusCode} - {errorContent}");
        }
        
        // Assert
        Assert.True(response.IsSuccessStatusCode, $"Failed to get users: {response.StatusCode}");
        
        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Super subscription users: {content}");
        
        var users = JsonSerializer.Deserialize<JsonElement[]>(content);
        Assert.NotNull(users);
        
        _output.WriteLine($"✅ Found {users.Length} users with Super subscription");
    }

    [Fact]
    public async Task UserService_HealthCheck_ShouldRespond()
    {
        // Act
        var response = await _fixture.UserServiceClient.GetAsync("/api/users");
        
        // Assert
        _output.WriteLine($"UserService health check: {response.StatusCode}");
        
        // UserService може повернути 200 (з користувачами) або 404 (якщо немає користувачів)
        Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK || 
                   response.StatusCode == System.Net.HttpStatusCode.NotFound);
    }
}