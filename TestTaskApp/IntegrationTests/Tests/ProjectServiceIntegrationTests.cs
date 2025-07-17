using System.Text;
using System.Text.Json;
using IntegrationTests.Infrastructure;
using Xunit.Abstractions;

namespace IntegrationTests.Tests;

[Collection("Integration Tests")]
public class ProjectServiceIntegrationTests : IClassFixture<SharedTestFixture>
{
    private readonly SharedTestFixture _fixture;
    private readonly ITestOutputHelper _output;

    public ProjectServiceIntegrationTests(SharedTestFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }

    [Fact]
    public async Task ProjectService_CreateProject_ShouldPersistInMongoDB()
    {
        // Arrange
        var createProjectRequest = new
        {
            UserId = 1,
            Name = "Integration Test Project",
            Charts = new[]
            {
                new
                {
                    Symbol = "EURUSD",
                    Timeframe = "H1",
                    Indicators = new[]
                    {
                        new { Name = "MA", Parameters = "period=20" }
                    }
                }
            }
        };

        // Act
        var createContent = new StringContent(JsonSerializer.Serialize(createProjectRequest), Encoding.UTF8, "application/json");
        var createResponse = await _fixture.ProjectServiceClient.PostAsync("/api/projects", createContent);

        // Debug output
        if (!createResponse.IsSuccessStatusCode)
        {
            var errorContent = await createResponse.Content.ReadAsStringAsync();
            _output.WriteLine($"❌ ProjectService error: {createResponse.StatusCode} - {errorContent}");
        }

        // Assert
        Assert.True(createResponse.IsSuccessStatusCode, $"Failed to create project: {createResponse.StatusCode}");
        
        var createResponseContent = await createResponse.Content.ReadAsStringAsync();
        _output.WriteLine($"✅ Project created: {createResponseContent}");
    }

    [Fact]
    public async Task ProjectService_GetAllProjects_ShouldReturnSeededData()
    {
        // Act
        var response = await _fixture.ProjectServiceClient.GetAsync("/api/projects");
        
        // Assert
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine($"❌ ProjectService error: {response.StatusCode} - {errorContent}");
        }
        
        Assert.True(response.IsSuccessStatusCode, $"Failed to get projects: {response.StatusCode}");
        
        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Projects response: {content}");
        
        var projects = JsonSerializer.Deserialize<JsonElement[]>(content);
        Assert.NotNull(projects);
        Assert.True(projects.Length > 0, "Should have seeded projects");
        
        _output.WriteLine($"✅ Found {projects.Length} projects");
    }

    [Fact]
    public async Task ProjectService_HealthCheck_ShouldRespond()
    {
        // Act
        var response = await _fixture.ProjectServiceClient.GetAsync("/api/projects");
        
        // Assert
        _output.WriteLine($"ProjectService health check: {response.StatusCode}");
        Assert.True(response.IsSuccessStatusCode);
    }
}