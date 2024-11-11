using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDo.Domain.Entities;

namespace ToDo.Infrastructure.Persistence.Configurations
{
    public class TenantUserConfiguration : IEntityTypeConfiguration<TenantUser>
    {
        public void Configure(EntityTypeBuilder<TenantUser> builder)
        {
            builder.HasKey(tu => new { tu.TenantId, tu.UserId });

            builder.HasOne(tu => tu.Tenant)
                .WithMany(t => t.TenantUsers)
                .HasForeignKey(tu => tu.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(tu => tu.User)
                .WithMany(u => u.TenantUsers)
                .HasForeignKey(tu => tu.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(tu => tu.Role)
                .IsRequired()
                .HasConversion<string>();
        }
    }
} 