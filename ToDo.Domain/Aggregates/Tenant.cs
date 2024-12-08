namespace ToDo.Domain.Aggregates;
using ToDo.Domain.Entities;
using ToDo.Domain.Events;
using ToDo.Domain.Exceptions;

public class Tenant : AggregateRoot<Guid>, IAggregateRoot
{
  public string Name { get; private set; }
  public DateTime CreatedAt { get; private set; }
  public DateTime UpdatedAt { get; private set; }

  private readonly List<TenantUser> _tenantUsers = new();
  public IReadOnlyCollection<TenantUser> TenantUsers => _tenantUsers.AsReadOnly();

  private Tenant() : base(Guid.Empty)
  {
    Name = string.Empty;
  }

  public static Tenant Create(string name, ApplicationUser creator)
  {
    if (creator == null)
      throw new ArgumentNullException(nameof(creator));

    var tenant = new Tenant
    {
      Id = Guid.NewGuid(),
      Name = name,
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow
    };

    var tenantUser = new TenantUser(tenant.Id, creator.Id, TenantRole.Owner);
    tenant._tenantUsers.Add(tenantUser);

    tenant.AddDomainEvent(new TenantCreatedDomainEvent(tenant.Id, name));
    tenant.AddDomainEvent(new UserAddedToTenantDomainEvent(tenant.Id, creator.Id, TenantRole.Owner));

    return tenant;
  }

  public void UpdateName(string newName)
  {
    Name = newName;
    UpdatedAt = DateTime.UtcNow;
  }

  public void AddUser(ApplicationUser user, TenantRole role)
  {
    var tenantUser = new TenantUser(Id, user.Id, role);
    _tenantUsers.Add(tenantUser);

    AddDomainEvent(new UserAddedToTenantDomainEvent(Id, user.Id, role));
  }

  public void RemoveUser(string userId)
  {
    var user = _tenantUsers.FirstOrDefault(u => u.UserId == userId);
    if (user == null)
      throw new DomainException("User not found in tenant");

    if (user.Role == TenantRole.Owner && _tenantUsers.Count(u => u.Role == TenantRole.Owner) == 1)
      throw new DomainException("Cannot remove the last owner");

    _tenantUsers.Remove(user);
    AddDomainEvent(new UserRemovedFromTenantDomainEvent(Id, userId));
  }

  public void UpdateUserRole(string userId, TenantRole newRole)
  {
    var user = _tenantUsers.FirstOrDefault(u => u.UserId == userId);
    if (user == null)
      throw new DomainException("User not found in tenant");

    if (user.Role == TenantRole.Owner &&
        newRole != TenantRole.Owner &&
        _tenantUsers.Count(u => u.Role == TenantRole.Owner) == 1)
      throw new DomainException("Cannot change role of the last owner");

    user.UpdateRole(newRole);
    AddDomainEvent(new UserRoleUpdatedInTenantDomainEvent(Id, userId, newRole));
  }

  public bool HasUser(string userId)
  {
    return _tenantUsers.Any(tu => tu.UserId == userId);
  }
}
