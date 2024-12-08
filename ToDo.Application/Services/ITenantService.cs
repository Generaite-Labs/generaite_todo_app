using ToDo.Domain.Aggregates;

namespace ToDo.Application.Services;

public interface ITenantService
{
  Task<Tenant> CreateTenantAsync(string name);
}
