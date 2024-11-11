namespace ToDo.Domain.Interfaces;

public interface ITenantContext
{
    Guid CurrentTenantId { get; }
} 