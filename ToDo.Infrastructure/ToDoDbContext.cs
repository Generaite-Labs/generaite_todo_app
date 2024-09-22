using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Entities;

namespace ToDo.Infrastructure;

public class TodoDbContext : IdentityDbContext<ApplicationUser>
{
  public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    // Add any additional model configurations here
  }

  // Add DbSet properties for your entities here as you create them
  // For example:
  // public DbSet<TodoItem> TodoItems { get; set; }
}