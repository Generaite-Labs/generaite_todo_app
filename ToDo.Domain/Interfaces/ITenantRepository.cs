using ToDo.Domain.Entities;

namespace ToDo.Domain.Interfaces
{
    public interface ITenantRepository : IBaseRepository<Tenant>
    {
        Task<Tenant?> GetByIdAsync(Guid id);
        Task<Tenant> AddAsync(Tenant tenant);
        Task UpdateAsync(Tenant tenant);
        Task DeleteAsync(Tenant tenant);
    }
} 