using Moq;
using ToDo.Application.Services;
using ToDo.Domain.Aggregates;
using ToDo.Domain.Entities;
using ToDo.Domain.Interfaces;
using ToDo.Application.Interfaces;

namespace ToDo.Tests.Application;

public class TenantServiceTests
{
    private readonly Mock<ITenantRepository> _tenantRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly TenantService _sut;

    public TenantServiceTests()
    {
        _tenantRepositoryMock = new Mock<ITenantRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _sut = new TenantService(
            _tenantRepositoryMock.Object,
            _currentUserServiceMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task CreateTenantAsync_WithValidData_CreatesTenant()
    {
        // Arrange
        var testUser = new User { Id = "user1", Email = "test@example.com" };
        var tenantName = "Test Tenant";

        _currentUserServiceMock.Setup(x => x.GetCurrentUser())
            .ReturnsAsync(testUser);

        // Act
        var result = await _sut.CreateTenantAsync(tenantName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(tenantName, result.Name);

        _tenantRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Tenant>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateTenantAsync_WithNoCurrentUser_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.GetCurrentUser())
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _sut.CreateTenantAsync("Test Tenant")
        );
    }

    [Fact]
    public async Task GetTenantsForUserAsync_WithValidUserId_ReturnsTenants()
    {
        // Arrange
        var userId = "user1";
        var expectedTenants = new List<Tenant>
        {
            Tenant.Create("Tenant 1", new User { Id = userId }),
            Tenant.Create("Tenant 2", new User { Id = userId })
        };

        _tenantRepositoryMock.Setup(x => x.GetTenantsForUserAsync(userId))
            .ReturnsAsync(expectedTenants);

        // Act
        var result = await _sut.GetTenantsForUserAsync(userId);

        // Assert
        Assert.Equal(expectedTenants.Count, result.Count);
        _tenantRepositoryMock.Verify(x => x.GetTenantsForUserAsync(userId), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task GetTenantsForUserAsync_WithInvalidUserId_ThrowsArgumentException(string userId)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _sut.GetTenantsForUserAsync(userId)
        );
    }

    [Fact]
    public async Task SwitchTenantAsync_WithValidAccess_SwitchesTenant()
    {
        // Arrange
        var testUser = new User { Id = "user1" };
        var tenantId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.GetCurrentUser())
            .ReturnsAsync(testUser);
        _tenantRepositoryMock.Setup(x => x.IsUserInTenantAsync(tenantId, testUser.Id))
            .ReturnsAsync(true);

        // Act
        await _sut.SwitchTenantAsync(tenantId);

        // Assert
        _currentUserServiceMock.Verify(x => x.SetCurrentTenantId(tenantId), Times.Once);
    }

    [Fact]
    public async Task SwitchTenantAsync_WithNoAccess_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var testUser = new User { Id = "user1" };
        var tenantId = Guid.NewGuid();

        _currentUserServiceMock.Setup(x => x.GetCurrentUser())
            .ReturnsAsync(testUser);
        _tenantRepositoryMock.Setup(x => x.IsUserInTenantAsync(tenantId, testUser.Id))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _sut.SwitchTenantAsync(tenantId)
        );
    }

    [Fact]
    public async Task SwitchTenantAsync_WithNoCurrentUser_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.GetCurrentUser())
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _sut.SwitchTenantAsync(Guid.NewGuid())
        );
    }
}
