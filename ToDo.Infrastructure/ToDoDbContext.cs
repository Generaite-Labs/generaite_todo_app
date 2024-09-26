using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Entities;

namespace ToDo.Infrastructure
{
  public class TodoDbContext : IdentityDbContext<ApplicationUser>
  {
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }

    public DbSet<TodoItem> TodoItems { get; set; } = null!;
    public DbSet<TodoItemList> TodoItemLists { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<TodoItem>(entity =>
      {
        entity.HasOne(t => t.User)
              .WithMany(u => u.TodoItems)
              .HasForeignKey(t => t.UserId)
              .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(t => t.TodoItemList)
              .WithMany(tl => tl.TodoItems)
              .HasForeignKey(t => t.TodoItemListId)
              .OnDelete(DeleteBehavior.SetNull);

        entity.Property(t => t.Title).HasMaxLength(200);
        entity.Property(t => t.Description).HasMaxLength(1000);
      });

      modelBuilder.Entity<TodoItemList>(entity =>
      {
        entity.HasOne(tl => tl.User)
              .WithMany(u => u.TodoItemLists)
              .HasForeignKey(tl => tl.UserId)
              .OnDelete(DeleteBehavior.Cascade);

        entity.Property(tl => tl.Name).HasMaxLength(100);
      });
    }
  }
}