using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Entities;
using ToDo.Domain.Interfaces;

namespace ToDo.Infrastructure.Repositories
{
    public class TenantRepository : BaseRepository<Tenant>, ITenantRepository
    {
        private readonly ApplicationDbContext _todoContext;

        public TenantRepository(ApplicationDbContext context) : base(context)
        {
            _todoContext = context;
        }

        public async Task<Tenant?> GetByIdAsync(Guid id)
        {
            return await _todoContext.Tenants
                .Include(t => t.TenantUsers)
                    .ThenInclude(tu => tu.User)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Tenant> AddAsync(Tenant tenant)
        {
            await _todoContext.Tenants.AddAsync(tenant);
            return tenant;
        }

        public Task UpdateAsync(Tenant tenant)
        {
            _todoContext.Entry(tenant).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Tenant tenant)
        {
            _todoContext.Tenants.Remove(tenant);
            return Task.CompletedTask;
        }
    }
} 