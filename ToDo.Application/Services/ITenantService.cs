using ToDo.Domain.Aggregates;

namespace ToDo.Application.Services;

public interface ITenantService
{
  Task<Tenant> CreateTenantAsync(string name);
  Task<IReadOnlyList<Tenant>> GetTenantsForUserAsync(string userId);
}
