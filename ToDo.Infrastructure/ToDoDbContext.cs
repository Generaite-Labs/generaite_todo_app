using Microsoft.EntityFrameworkCore;

namespace ToDo.Infrastructure;

public class TodoDbContext : DbContext
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