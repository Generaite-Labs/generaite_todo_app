namespace ToDo.Domain.Entities;

public interface ITenantScoped
{
    Guid TenantId { get; }
} 