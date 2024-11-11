using Microsoft.AspNetCore.Mvc;
using ToDo.Application.Services;
using ToDo.Domain.Aggregates;

namespace ToDo.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantService _tenantService;

        public TenantsController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        [HttpPost]
        public async Task<ActionResult<TenantResponse>> Create([FromBody] CreateTenantRequest request)
        {
            var tenant = await _tenantService.CreateTenantAsync(request.Name);
            return Ok(new TenantResponse(tenant.Id, tenant.Name, tenant.CreatedAt));
        }
    }

    public record CreateTenantRequest(string Name);
    public record TenantResponse(Guid Id, string Name, DateTime CreatedAt);
} 