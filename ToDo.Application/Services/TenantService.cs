using ToDo.Domain.Aggregates;
using ToDo.Domain.Interfaces;
using ToDo.Application.Interfaces;

namespace ToDo.Application.Services;

public class TenantService : ITenantService
{
  private readonly ITenantRepository _tenantRepository;
  private readonly ICurrentUserService _currentUserService;
  private readonly IUnitOfWork _unitOfWork;

  public TenantService(
      ITenantRepository tenantRepository,
      ICurrentUserService currentUserService,
      IUnitOfWork unitOfWork)
  {
    _tenantRepository = tenantRepository;
    _currentUserService = currentUserService;
    _unitOfWork = unitOfWork;
  }

  public async Task<Tenant> CreateTenantAsync(string name)
  {
    var currentUser = await _currentUserService.GetCurrentUser()
        ?? throw new UnauthorizedAccessException("User not found");

    var tenant = Tenant.Create(name, currentUser);

    await _tenantRepository.AddAsync(tenant);
    await _unitOfWork.SaveChangesAsync();

    return tenant;
  }

  public async Task<IReadOnlyList<Tenant>> GetTenantsForUserAsync(string userId)
  {
    if (string.IsNullOrEmpty(userId))
      throw new ArgumentException("User ID cannot be empty", nameof(userId));

    var tenants = await _tenantRepository.GetTenantsForUserAsync(userId);
    return tenants.ToList();
  }

  public async Task SwitchTenantAsync(Guid tenantId)
  {
    var currentUser = await _currentUserService.GetCurrentUser()
        ?? throw new UnauthorizedAccessException("User not found");

    var hasAccess = await _tenantRepository.IsUserInTenantAsync(tenantId, currentUser.Id);
    if (!hasAccess)
      throw new UnauthorizedAccessException("User does not have access to this tenant");

    await _currentUserService.SetCurrentTenantId(tenantId);
  }
}
