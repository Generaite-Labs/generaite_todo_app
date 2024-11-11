using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Entities;
using ToDo.Infrastructure.Persistence.Configurations;

namespace ToDo.Infrastructure
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
			: base(options)
		{
		}

		public DbSet<TodoItem> TodoItems { get; set; } = null!;
		public DbSet<Tenant> Tenants { get; set; } = null!;
		public DbSet<TenantUser> TenantUsers { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.ApplyConfiguration(new TodoItemConfiguration());
			modelBuilder.ApplyConfiguration(new TenantConfiguration());
			modelBuilder.ApplyConfiguration(new TenantUserConfiguration());
		}
	}
}