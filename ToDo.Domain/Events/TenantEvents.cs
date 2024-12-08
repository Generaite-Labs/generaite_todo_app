namespace ToDo.Domain.Events;

using ToDo.Domain.Interfaces;
using ToDo.Domain.Entities;

public record TenantCreatedDomainEvent : IDomainEvent
{
  public Guid Id { get; } = Guid.NewGuid();
  public DateTime OccurredOn { get; } = DateTime.UtcNow;
  public long Version { get; init; }
  public Guid AggregateId { get; }
  public string AggregateType { get; } = "Tenant";

  public Guid TenantId { get; }
  public string Name { get; }

  public TenantCreatedDomainEvent(Guid tenantId, string name)
  {
    TenantId = tenantId;
    Name = name;
    AggregateId = tenantId;
  }
}

public record UserAddedToTenantDomainEvent : IDomainEvent
{
  public Guid Id { get; } = Guid.NewGuid();
  public DateTime OccurredOn { get; } = DateTime.UtcNow;
  public long Version { get; init; }
  public Guid AggregateId { get; }
  public string AggregateType { get; } = "Tenant";

  public Guid TenantId { get; }
  public string UserId { get; }
  public TenantRole Role { get; }

  public UserAddedToTenantDomainEvent(Guid tenantId, string userId, TenantRole role)
  {
    TenantId = tenantId;
    UserId = userId;
    Role = role;
    AggregateId = tenantId;
  }
}

public record UserRemovedFromTenantDomainEvent : IDomainEvent
{
  public Guid Id { get; } = Guid.NewGuid();
  public DateTime OccurredOn { get; } = DateTime.UtcNow;
  public long Version { get; init; }
  public Guid AggregateId { get; }
  public string AggregateType { get; } = "Tenant";

  public Guid TenantId { get; }
  public string UserId { get; }

  public UserRemovedFromTenantDomainEvent(Guid tenantId, string userId)
  {
    TenantId = tenantId;
    UserId = userId;
    AggregateId = tenantId;
  }
}

public record UserRoleUpdatedInTenantDomainEvent : IDomainEvent
{
  public Guid Id { get; } = Guid.NewGuid();
  public DateTime OccurredOn { get; } = DateTime.UtcNow;
  public long Version { get; init; }
  public Guid AggregateId { get; }
  public string AggregateType { get; } = "Tenant";

  public Guid TenantId { get; }
  public string UserId { get; }
  public TenantRole NewRole { get; }

  public UserRoleUpdatedInTenantDomainEvent(Guid tenantId, string userId, TenantRole newRole)
  {
    TenantId = tenantId;
    UserId = userId;
    NewRole = newRole;
    AggregateId = tenantId;
  }
}
