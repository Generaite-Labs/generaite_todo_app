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
} 