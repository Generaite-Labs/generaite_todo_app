namespace ToDo.Domain.Exceptions;
public class CannotRemoveLastTenantOwnerException : DomainException
{
    public CannotRemoveLastTenantOwnerException(Guid tenantId, string userId) 
        : base($"Cannot remove user {userId} as they are the last owner of tenant {tenantId}")
    {
        TenantId = tenantId;
        UserId = userId;
    }

    public Guid TenantId { get; }
    public string UserId { get; }
}