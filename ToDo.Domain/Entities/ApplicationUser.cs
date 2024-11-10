using Microsoft.AspNetCore.Identity;

namespace ToDo.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        
        // Navigation properties with private setters for better encapsulation
        public virtual ICollection<TenantUser> TenantUsers { get; private set; } = new List<TenantUser>();
        public virtual ICollection<TodoItem> TodoItems { get; private set; } = new List<TodoItem>();

        public ApplicationUser() 
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateFullName(string? newFullName)
        {
            FullName = newFullName;
            UpdatedAt = DateTime.UtcNow;
        }

        // Helper method to check tenant membership
        public bool IsMemberOfTenant(Guid tenantId)
        {
            return TenantUsers.Any(tu => tu.TenantId == tenantId);
        }

        // Helper method to get user's role in a specific tenant
        public TenantRole? GetRoleInTenant(Guid tenantId)
        {
            return TenantUsers
                .FirstOrDefault(tu => tu.TenantId == tenantId)
                ?.Role;
        }
    }
}