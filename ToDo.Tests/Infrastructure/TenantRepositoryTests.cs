using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Entities;
using ToDo.Domain.Aggregates;
using ToDo.Infrastructure.Repositories;
using Xunit;

namespace ToDo.Infrastructure.Tests.Repositories
{
    public class TenantRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly TenantRepository _repository;

        public TenantRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new TenantRepository(_context);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingTenant_ReturnsTenant()
        {
            // Arrange
            var owner = new ApplicationUser { UserName = "owner@test.com", Email = "owner@test.com" };
            var tenant = Tenant.Create("Test Tenant", owner);
            var user = new ApplicationUser { UserName = "test@test.com", Email = "test@test.com" };
            tenant.AddUser(user, TenantRole.Admin);
            
            await _context.Users.AddAsync(owner);
            await _context.Users.AddAsync(user);
            await _context.Tenants.AddAsync(tenant);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(tenant.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Tenant", result.Name);
            Assert.Equal(2, result.TenantUsers.Count);  // Owner + Added user
            Assert.Contains(result.TenantUsers, tu => tu.UserId == user.Id && tu.Role == TenantRole.Admin);
            Assert.Contains(result.TenantUsers, tu => tu.UserId == owner.Id && tu.Role == TenantRole.Owner);
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingTenant_ReturnsNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_ValidTenant_AddsTenantToDatabase()
        {
            // Arrange
            var owner = new ApplicationUser { UserName = "owner@test.com", Email = "owner@test.com" };
            await _context.Users.AddAsync(owner);
            await _context.SaveChangesAsync();

            var tenant = Tenant.Create("New Tenant", owner);

            // Act
            var result = await _repository.AddAsync(tenant);
            await _context.SaveChangesAsync();

            // Assert
            var savedTenant = await _context.Tenants.FindAsync(tenant.Id);
            Assert.NotNull(savedTenant);
            Assert.Equal("New Tenant", savedTenant.Name);
            Assert.Single(savedTenant.TenantUsers);
            Assert.Equal(owner.Id, savedTenant.TenantUsers.First().UserId);
            Assert.Equal(TenantRole.Owner, savedTenant.TenantUsers.First().Role);
        }

        [Fact]
        public async Task UpdateAsync_ExistingTenant_UpdatesTenantInDatabase()
        {
            // Arrange
            var owner = new ApplicationUser { UserName = "owner@test.com", Email = "owner@test.com" };
            await _context.Users.AddAsync(owner);
            await _context.SaveChangesAsync();

            var tenant = Tenant.Create("Original Name", owner);
            await _context.Tenants.AddAsync(tenant);
            await _context.SaveChangesAsync();

            // Act
            tenant.UpdateName("Updated Name");
            await _repository.UpdateAsync(tenant);
            await _context.SaveChangesAsync();

            // Assert
            var updatedTenant = await _context.Tenants.FindAsync(tenant.Id);
            Assert.NotNull(updatedTenant);
            Assert.Equal("Updated Name", updatedTenant.Name);
        }

        [Fact]
        public async Task DeleteAsync_ExistingTenant_RemovesTenantFromDatabase()
        {
            // Arrange
            var owner = new ApplicationUser { UserName = "owner@test.com", Email = "owner@test.com" };
            await _context.Users.AddAsync(owner);
            await _context.SaveChangesAsync();

            var tenant = Tenant.Create("To Be Deleted", owner);
            await _context.Tenants.AddAsync(tenant);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteAsync(tenant.Id);
            await _context.SaveChangesAsync();

            // Assert
            var deletedTenant = await _context.Tenants.FindAsync(tenant.Id);
            Assert.Null(deletedTenant);
        }

        [Fact]
        public async Task GetByIdAsync_TenantWithMultipleUsers_LoadsTenantUsersAndUsers()
        {
            // Arrange
            var owner = new ApplicationUser { UserName = "owner@test.com", Email = "owner@test.com" };
            var tenant = Tenant.Create("Multi-User Tenant", owner);
            var user1 = new ApplicationUser { UserName = "user1@test.com", Email = "user1@test.com" };
            var user2 = new ApplicationUser { UserName = "user2@test.com", Email = "user2@test.com" };
            
            tenant.AddUser(user1, TenantRole.Admin);
            tenant.AddUser(user2, TenantRole.Member);

            await _context.Users.AddRangeAsync(owner, user1, user2);
            await _context.Tenants.AddAsync(tenant);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(tenant.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.TenantUsers.Count);  // Owner + 2 added users
            Assert.Contains(result.TenantUsers, tu => tu.UserId == user1.Id && tu.Role == TenantRole.Admin);
            Assert.Contains(result.TenantUsers, tu => tu.UserId == user2.Id && tu.Role == TenantRole.Member);
            Assert.Contains(result.TenantUsers, tu => tu.UserId == owner.Id && tu.Role == TenantRole.Owner);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
} 