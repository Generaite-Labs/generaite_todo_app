namespace ToDo.Domain.Entities;

public abstract class TenantScoped
{
    public Guid TenantId { get; protected set; }

    protected TenantScoped(Guid tenantId)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("TenantId cannot be empty", nameof(tenantId));
            
        TenantId = tenantId;
    }

    protected TenantScoped() { }
} 