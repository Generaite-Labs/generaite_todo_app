using ToDo.Domain.Entities;

namespace ToDo.Application.Interfaces;

public interface ICurrentUserService
{
  Task<ApplicationUser?> GetCurrentUser();
    Task<Guid?> GetCurrentTenantId();
    Task SetCurrentTenantId(Guid tenantId);
}
