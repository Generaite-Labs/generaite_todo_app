using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDo.Domain.Aggregates;

namespace ToDo.Infrastructure.Persistence.Configurations
{
  public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
  {
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
      builder.Property(t => t.Name)
          .IsRequired()
          .HasMaxLength(200);
    }
  }
}
