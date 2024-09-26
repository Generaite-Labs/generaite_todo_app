using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Entities;

namespace ToDo.Infrastructure
{
  public class TodoDbContext : IdentityDbContext<ApplicationUser>
  {
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }

    public DbSet<TodoItem> TodoItems { get; set; }
    public DbSet<TodoItemList> TodoItemLists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<TodoItem>()
          .HasOne(t => t.User)
          .WithMany(u => u.TodoItems)
          .HasForeignKey(t => t.UserId)
          .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<TodoItem>()
          .HasOne(t => t.TodoItemList)
          .WithMany(tl => tl.TodoItems)
          .HasForeignKey(t => t.TodoItemListId)
          .OnDelete(DeleteBehavior.SetNull);

      modelBuilder.Entity<TodoItemList>()
          .HasOne(tl => tl.User)
          .WithMany(u => u.TodoItemLists)
          .HasForeignKey(tl => tl.UserId)
          .OnDelete(DeleteBehavior.Cascade);
    }
  }
}