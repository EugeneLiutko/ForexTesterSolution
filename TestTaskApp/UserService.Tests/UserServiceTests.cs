using Moq;
using UserService.Application.Repositories;
using UserService.Application.Services;
using UserService.Domain.Entities;

namespace UserService.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ISubscriptionService> _mockSubscriptionService;
    private readonly UserService.Application.Services.UserService _userService;
    private readonly Mock<Microsoft.Extensions.Logging.ILogger<UserService.Application.Services.UserService>> _mockLogger;
    

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockSubscriptionService = new Mock<ISubscriptionService>();
        _mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<UserService.Application.Services.UserService>>();
        _userService = new UserService.Application.Services.UserService(_mockUserRepository.Object, _mockSubscriptionService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenUserExists_ReturnsUser()
    {
        // Arrange
        var userId = 1;
        var expectedUser = new User("John Doe", "john@example.com", 1);
        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUser, result);
        _mockUserRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenUserNotExists_ThrowsKeyNotFoundException()
    {
        // Arrange
        var userId = 1;
        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _userService.GetUserByIdAsync(userId));
        
        Assert.Contains($"User with ID {userId} not found", exception.Message);
    }

    [Fact]
    public async Task UpdateUserAsync_WhenUserExists_UpdatesUser()
    {
        // Arrange
        var userId = 1;
        var newName = "Jane Doe";
        var newEmail = "jane@example.com";
        var existingUser = new User("John Doe", "john@example.com", 1);
        
        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);
        _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        // Act
        await _userService.UpdateUserAsync(userId, newName, newEmail);

        // Assert
        Assert.Equal(newName, existingUser.Name);
        Assert.Equal(newEmail, existingUser.Email);
        _mockUserRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
        _mockUserRepository.Verify(x => x.UpdateAsync(existingUser), Times.Once);
    }
}