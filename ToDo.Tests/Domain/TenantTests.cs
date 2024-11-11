namespace ToDo.Domain.Tests.Aggregates;

using ToDo.Domain.Aggregates;
using ToDo.Domain.Entities;
using ToDo.Domain.Events;
using ToDo.Domain.Exceptions;

public class TenantTests
{
    private readonly ApplicationUser _creator;

    public TenantTests()
    {
        _creator = new ApplicationUser { Id = "user-1", UserName = "test@example.com" };
    }

    [Fact]
    public void Create_WithValidData_CreatesNewTenant()
    {
        // Act
        var tenant = Tenant.Create("Test Tenant", _creator);

        // Assert
        Assert.NotEqual(Guid.Empty, tenant.Id);
        Assert.Equal("Test Tenant", tenant.Name);
        Assert.Single(tenant.TenantUsers);
        Assert.Contains(tenant.TenantUsers, tu => 
            tu.UserId == _creator.Id && 
            tu.Role == TenantRole.Owner);

        // Verify domain events
        Assert.Equal(2, tenant.DomainEvents.Count);
        Assert.Contains(tenant.DomainEvents, e => e is TenantCreatedDomainEvent);
        Assert.Contains(tenant.DomainEvents, e => e is UserAddedToTenantDomainEvent);
    }

    [Fact]
    public void Create_WithNullCreator_ThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => 
            Tenant.Create("Test Tenant", null!));
        
        Assert.Equal("creator", exception.ParamName);
    }

    [Fact]
    public void AddUser_WhenUserNotInTenant_AddsUser()
    {
        // Arrange
        var tenant = Tenant.Create("Test Tenant", _creator);
        var newUser = new ApplicationUser { Id = "user-2" };
        tenant.ClearDomainEvents(); // Clear events from creation

        // Act
        tenant.AddUser(newUser, TenantRole.Member);

        // Assert
        Assert.Equal(2, tenant.TenantUsers.Count);
        var addedUser = Assert.Single(tenant.TenantUsers, tu => tu.UserId == newUser.Id);
        Assert.Equal(TenantRole.Member, addedUser.Role);

        // Verify domain event
        var @event = Assert.Single(tenant.DomainEvents);
        var addedEvent = Assert.IsType<UserAddedToTenantDomainEvent>(@event);
        Assert.Equal(newUser.Id, addedEvent.UserId);
        Assert.Equal(TenantRole.Member, addedEvent.Role);
    }

    [Fact]
    public void RemoveUser_WhenUserExists_RemovesUser()
    {
        // Arrange
        var tenant = Tenant.Create("Test Tenant", _creator);
        var member = new ApplicationUser { Id = "user-2" };
        tenant.AddUser(member, TenantRole.Member);
        tenant.ClearDomainEvents();

        // Act
        tenant.RemoveUser(member.Id);

        // Assert
        Assert.Single(tenant.TenantUsers);
        Assert.DoesNotContain(tenant.TenantUsers, tu => tu.UserId == member.Id);

        // Verify domain event
        var @event = Assert.Single(tenant.DomainEvents);
        var removedEvent = Assert.IsType<UserRemovedFromTenantDomainEvent>(@event);
        Assert.Equal(member.Id, removedEvent.UserId);
    }

    [Fact]
    public void RemoveUser_WhenLastOwner_ThrowsDomainException()
    {
        // Arrange
        var tenant = Tenant.Create("Test Tenant", _creator);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            tenant.RemoveUser(_creator.Id));
        
        Assert.Equal("Cannot remove the last owner", exception.Message);
    }

    [Fact]
    public void UpdateUserRole_ChangingLastOwner_ThrowsDomainException()
    {
        // Arrange
        var tenant = Tenant.Create("Test Tenant", _creator);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            tenant.UpdateUserRole(_creator.Id, TenantRole.Member));
        
        Assert.Equal("Cannot change role of the last owner", exception.Message);
    }

    [Fact]
    public void UpdateUserRole_WithValidRole_UpdatesRole()
    {
        // Arrange
        var tenant = Tenant.Create("Test Tenant", _creator);
        var member = new ApplicationUser { Id = "user-2" };
        tenant.AddUser(member, TenantRole.Member);
        tenant.ClearDomainEvents();

        // Act
        tenant.UpdateUserRole(member.Id, TenantRole.Admin);

        // Assert
        var updatedUser = Assert.Single(tenant.TenantUsers, tu => tu.UserId == member.Id);
        Assert.Equal(TenantRole.Admin, updatedUser.Role);

        // Verify domain event
        var @event = Assert.Single(tenant.DomainEvents);
        var updatedEvent = Assert.IsType<UserRoleUpdatedInTenantDomainEvent>(@event);
        Assert.Equal(member.Id, updatedEvent.UserId);
        Assert.Equal(TenantRole.Admin, updatedEvent.NewRole);
    }

    [Fact]
    public void HasUser_WithExistingUser_ReturnsTrue()
    {
        // Arrange
        var tenant = Tenant.Create("Test Tenant", _creator);

        // Act & Assert
        Assert.True(tenant.HasUser(_creator.Id));
    }

    [Fact]
    public void HasUser_WithNonExistingUser_ReturnsFalse()
    {
        // Arrange
        var tenant = Tenant.Create("Test Tenant", _creator);

        // Act & Assert
        Assert.False(tenant.HasUser("non-existing-user"));
    }
} 