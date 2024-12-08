using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Aggregates;
using ToDo.Domain.Interfaces;

namespace ToDo.Infrastructure.Repositories
{
  public class TenantRepository : BaseRepository<Tenant, Guid>, ITenantRepository
  {
    public TenantRepository(ApplicationDbContext context) : base(context) { }

    public override async Task<Tenant?> GetByIdAsync(Guid id)
    {
      return await BaseQuery
          .Include(t => t.TenantUsers)
          .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Tenant>> GetTenantsForUserAsync(string userId)
    {
      return await BaseQuery
          .Include(t => t.TenantUsers)
          .Where(t => t.TenantUsers.Any(tu => tu.UserId == userId))
          .ToListAsync();
    }

    public async Task<bool> IsUserInTenantAsync(Guid tenantId, string userId)
    {
      return await BaseQuery
          .AnyAsync(t => t.Id == tenantId &&
                        t.TenantUsers.Any(tu => tu.UserId == userId));
    }
  }
}
