using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Entities;
using ToDo.Domain.ValueObjects;

namespace ToDo.Infrastructure
{
    public class TodoDbContext : IdentityDbContext<ApplicationUser>
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options)
            : base(options)
        {
        }

        public DbSet<TodoItem> TodoItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TodoItem>(entity =>
                {
                    entity.HasOne(t => t.User)
                          .WithMany(u => u.TodoItems)
                          .HasForeignKey(t => t.UserId)
                          .OnDelete(DeleteBehavior.Cascade);

                    entity.Property(t => t.Title).HasMaxLength(200);
                    entity.Property(t => t.Description).HasMaxLength(1000);

                    entity.Property(t => t.Status)
                        .HasConversion(
                            v => v.Value,
                            v => TodoItemStatus.FromString(v))
                        .HasMaxLength(20)
                        .IsRequired();
                });
        }
    }
}