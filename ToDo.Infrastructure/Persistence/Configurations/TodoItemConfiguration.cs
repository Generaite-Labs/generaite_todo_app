using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDo.Domain.Entities;
using ToDo.Domain.ValueObjects;

namespace ToDo.Infrastructure.Persistence.Configurations
{
  public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
  {
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
      builder.HasOne(t => t.User)
            .WithMany(u => u.TodoItems)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

      builder.Property(t => t.Title)
            .HasMaxLength(200);

      builder.Property(t => t.Description)
            .HasMaxLength(1000);

      builder.Property(t => t.Status)
            .HasConversion(
                v => v.Value,
                v => TodoItemStatus.FromString(v))
            .HasMaxLength(20)
            .IsRequired();
    }
  }
}
