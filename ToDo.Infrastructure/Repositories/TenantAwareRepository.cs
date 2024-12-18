using ToDo.Domain.Entities;
using ToDo.Domain.Interfaces;

namespace ToDo.Infrastructure.Repositories;

public abstract class TenantAwareRepository<T> : BaseRepository<T, Guid>
    where T : class, ITenantScoped
{
  protected readonly ApplicationDbContext _dbContext;
  protected readonly ITenantContext _tenantContext;

  protected TenantAwareRepository(ApplicationDbContext dbContext, ITenantContext tenantContext)
      : base(dbContext)
  {
    _dbContext = dbContext;
    _tenantContext = tenantContext;
  }

  protected override IQueryable<T> BaseQuery => _dbContext.Set<T>()
      .Where(e => e.TenantId == _tenantContext.CurrentTenantId);
}
