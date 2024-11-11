namespace ToDo.Domain.Entities;

public class TenantUser : AggregateEntity<Guid, Guid>, ITenantScoped
{
    public string UserId { get; private set; }
    public Guid TenantId { get; private set; }
    public TenantRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    public virtual Tenant? Tenant { get; private set; }
    public virtual ApplicationUser? User { get; private set; }

    private TenantUser() : base(Guid.Empty, Guid.Empty) 
    { 
        UserId = string.Empty;
        TenantId = Guid.Empty;
        Role = TenantRole.Member;
        CreatedAt = DateTime.UtcNow;
    } 

    public TenantUser(Guid tenantId, string userId, TenantRole role) 
        : base(Guid.NewGuid(), tenantId)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        TenantId = tenantId;
        UserId = userId;
        Role = role;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateRole(TenantRole newRole)
    {
        Role = newRole;
    }
} 

public enum TenantRole
{
    Owner = 1,
    Admin = 2,
    Member = 3
} 