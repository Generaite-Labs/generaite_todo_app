using ToDo.Domain.Aggregates;

namespace ToDo.Domain.Interfaces
{
  public interface ITenantRepository : IBaseRepository<Tenant, Guid>
  {
    Task<IEnumerable<Tenant>> GetTenantsForUserAsync(string userId);
    Task<bool> IsUserInTenantAsync(Guid tenantId, string userId);
  }
}
