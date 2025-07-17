using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using ProjectService.Application.DTOs;
using ProjectService.Application.Repositories;
using ProjectService.Domain.Entities;
using ProjectService.Domain.Enums;
using Xunit;

namespace ProjectService.Tests.Tests;

public class ProjectServiceTests
{
    private readonly Mock<IProjectRepository> _mockProjectRepository;
    private readonly Mock<ILogger<ProjectService.Application.Services.ProjectService>> _mockLogger;
    private readonly ProjectService.Application.Services.ProjectService _projectService;

    public ProjectServiceTests()
    {
        _mockProjectRepository = new Mock<IProjectRepository>();
        _mockLogger = new Mock<ILogger<ProjectService.Application.Services.ProjectService>>();
        _projectService =
            new ProjectService.Application.Services.ProjectService(_mockProjectRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetProjectByIdAsync_ValidId_ReturnsProjectDto()
    {
        // Arrange
        var projectId = ObjectId.GenerateNewId();
        var project = new Project
        {
            Id = projectId,
            UserId = 1,
            Name = "Test Project",
            Charts = new List<Chart>
            {
                new Chart
                {
                    Symbol = Symbol.EURUSD,
                    Timeframe = Timeframe.M5,
                    Indicators = new List<Indicator>
                    {
                        new Indicator { Name = IndicatorName.MA, Parameters = "period=20" }
                    }
                }
            }
        };

        _mockProjectRepository.Setup(r => r.GetByIdAsync(projectId))
            .ReturnsAsync(project);

        // Act
        var result = await _projectService.GetProjectByIdAsync(projectId.ToString());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(projectId.ToString(), result.Id);
        Assert.Equal("Test Project", result.Name);
        Assert.Equal(1, result.UserId);
        Assert.Single(result.Charts);
        Assert.Equal("EURUSD", result.Charts[0].Symbol);
        Assert.Equal("M5", result.Charts[0].Timeframe);
        Assert.Single(result.Charts[0].Indicators);
        Assert.Equal("MA", result.Charts[0].Indicators[0].Name);
    }

    [Fact]
    public async Task GetProjectByIdAsync_InvalidId_ThrowsArgumentException()
    {
        // Arrange
        var invalidId = "invalid-id";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _projectService.GetProjectByIdAsync(invalidId));
    }

    [Fact]
    public async Task GetProjectByIdAsync_ProjectNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var projectId = ObjectId.GenerateNewId();
        _mockProjectRepository.Setup(r => r.GetByIdAsync(projectId))
            .ReturnsAsync((Project)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _projectService.GetProjectByIdAsync(projectId.ToString()));
    }

    [Fact]
    public async Task CreateProjectAsync_ValidRequest_ReturnsProjectDto()
    {
        // Arrange
        var request = new CreateProjectRequest(
            UserId: 1,
            Name: "New Project",
            Charts: new List<ChartDto>
            {
                new ChartDto(
                    Symbol: "USDJPY",
                    Timeframe: "H1",
                    Indicators: new List<IndicatorDto>
                    {
                        new IndicatorDto("RSI", "period=14")
                    }
                )
            }
        );

        _mockProjectRepository.Setup(r => r.AddAsync(It.IsAny<Project>()))
            .Returns(Task.CompletedTask)
            .Callback<Project>(p => p.Id = ObjectId.GenerateNewId());

        // Act
        var result = await _projectService.CreateProjectAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Project", result.Name);
        Assert.Equal(1, result.UserId);
        Assert.Single(result.Charts);
        Assert.Equal("USDJPY", result.Charts[0].Symbol);
        Assert.Equal("H1", result.Charts[0].Timeframe);
        Assert.Single(result.Charts[0].Indicators);
        Assert.Equal("RSI", result.Charts[0].Indicators[0].Name);

        _mockProjectRepository.Verify(r => r.AddAsync(It.IsAny<Project>()), Times.Once);
    }

    [Fact]
    public async Task CreateProjectAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _projectService.CreateProjectAsync(null));
    }

    [Fact]
    public async Task CreateProjectAsync_EmptyName_ThrowsArgumentException()
    {
        // Arrange
        var request = new CreateProjectRequest(
            UserId: 1,
            Name: "",
            Charts: new List<ChartDto>()
        );

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _projectService.CreateProjectAsync(request));
    }

    [Fact]
    public async Task CreateProjectAsync_InvalidUserId_ThrowsArgumentException()
    {
        // Arrange
        var request = new CreateProjectRequest(
            UserId: 0,
            Name: "Test Project",
            Charts: new List<ChartDto>()
        );

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _projectService.CreateProjectAsync(request));
    }

    [Fact]
    public async Task GetProjectsByUserIdAsync_ValidUserId_ReturnsProjectDtos()
    {
        // Arrange
        var userId = 1;
        var projects = new List<Project>
        {
            new Project
            {
                Id = ObjectId.GenerateNewId(), UserId = userId, Name = "Project 1", Charts = new List<Chart>()
            },
            new Project
            {
                Id = ObjectId.GenerateNewId(), UserId = userId, Name = "Project 2", Charts = new List<Chart>()
            }
        };

        _mockProjectRepository.Setup(r => r.GetByUserIdAsync(userId))
            .ReturnsAsync(projects);

        // Act
        var result = await _projectService.GetProjectsByUserIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, p => Assert.Equal(userId, p.UserId));
    }

    [Fact]
    public async Task GetProjectsByUserIdAsync_InvalidUserId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _projectService.GetProjectsByUserIdAsync(0));
        await Assert.ThrowsAsync<ArgumentException>(() => _projectService.GetProjectsByUserIdAsync(-1));
    }

    [Fact]
    public async Task UpdateProjectAsync_ValidRequest_ReturnsUpdatedProjectDto()
    {
        // Arrange
        var projectId = ObjectId.GenerateNewId();
        var existingProject = new Project
        {
            Id = projectId,
            UserId = 1,
            Name = "Old Name",
            Charts = new List<Chart>()
        };

        var updateRequest = new UpdateProjectRequest(
            Name: "Updated Name",
            Charts: new List<ChartDto>
            {
                new ChartDto("EURUSD", "M1", new List<IndicatorDto>())
            }
        );

        _mockProjectRepository.Setup(r => r.GetByIdAsync(projectId))
            .ReturnsAsync(existingProject);
        _mockProjectRepository.Setup(r => r.UpdateAsync(It.IsAny<Project>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _projectService.UpdateProjectAsync(projectId.ToString(), updateRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
        Assert.Single(result.Charts);
        Assert.Equal("EURUSD", result.Charts[0].Symbol);

        _mockProjectRepository.Verify(r => r.UpdateAsync(It.IsAny<Project>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProjectAsync_ProjectNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var projectId = ObjectId.GenerateNewId();
        var updateRequest = new UpdateProjectRequest("Updated Name", new List<ChartDto>());

        _mockProjectRepository.Setup(r => r.GetByIdAsync(projectId))
            .ReturnsAsync((Project)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _projectService.UpdateProjectAsync(projectId.ToString(), updateRequest));
    }

    [Fact]
    public async Task DeleteProjectAsync_ValidId_CallsRepositoryDelete()
    {
        // Arrange
        var projectId = ObjectId.GenerateNewId();
        var existingProject = new Project
        {
            Id = projectId,
            UserId = 1,
            Name = "Test Project",
            Charts = new List<Chart>()
        };

        _mockProjectRepository.Setup(r => r.GetByIdAsync(projectId))
            .ReturnsAsync(existingProject);
        _mockProjectRepository.Setup(r => r.DeleteAsync(projectId))
            .Returns(Task.CompletedTask);

        // Act
        await _projectService.DeleteProjectAsync(projectId.ToString());

        // Assert
        _mockProjectRepository.Verify(r => r.DeleteAsync(projectId), Times.Once);
    }

    [Fact]
    public async Task DeleteProjectAsync_ProjectNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var projectId = ObjectId.GenerateNewId();
        _mockProjectRepository.Setup(r => r.GetByIdAsync(projectId))
            .ReturnsAsync((Project)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _projectService.DeleteProjectAsync(projectId.ToString()));
    }

    [Fact]
    public async Task GetAllProjectsAsync_ReturnsAllProjects()
    {
        // Arrange
        var projects = new List<Project>
        {
            new Project { Id = ObjectId.GenerateNewId(), UserId = 1, Name = "Project 1", Charts = new List<Chart>() },
            new Project { Id = ObjectId.GenerateNewId(), UserId = 2, Name = "Project 2", Charts = new List<Chart>() },
            new Project { Id = ObjectId.GenerateNewId(), UserId = 3, Name = "Project 3", Charts = new List<Chart>() }
        };

        _mockProjectRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(projects);

        // Act
        var result = await _projectService.GetAllProjectsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Contains(result, p => p.Name == "Project 1");
        Assert.Contains(result, p => p.Name == "Project 2");
        Assert.Contains(result, p => p.Name == "Project 3");
    }
}