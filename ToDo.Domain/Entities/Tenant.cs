namespace ToDo.Domain.Entities;

public class Tenant : AggregateRoot<Guid>, IAggregateRoot
{
    public required string Name { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    
    private readonly List<TenantUser> _tenantUsers = new();
    public IReadOnlyCollection<TenantUser> TenantUsers => _tenantUsers.AsReadOnly();

    private Tenant() : base(Guid.Empty) { } // For EF Core

    public Tenant(string name) : base(Guid.NewGuid())
    {
        Name = name;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Name cannot be empty", nameof(newName));
            
        Name = newName;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddUser(ApplicationUser user, TenantRole role)
    {
        if (_tenantUsers.Any(tu => tu.UserId == user.Id))
            throw new InvalidOperationException("User is already a member of this tenant");
            
        var tenantUser = new TenantUser(Id, user.Id, role);
        _tenantUsers.Add(tenantUser);
    }

    public bool HasUser(string userId)
    {
        return _tenantUsers.Any(tu => tu.UserId == userId);
    }
} 