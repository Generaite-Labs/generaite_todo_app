using ToDo.Core.DTOs;
using ToDo.Application.Services;
using Microsoft.AspNetCore.OpenApi;

namespace ToDo.Web.Endpoints;

public static class TenantEndpoints
{
  public static void MapTenantEndpoints(this IEndpointRouteBuilder app)
  {
    app.MapPost("/api/tenants", async (
        CreateTenantDto request,
        ITenantService tenantService) =>
    {
      try
      {
        var tenant = await tenantService.CreateTenantAsync(request.Name);
        return Results.Created($"/api/tenants/{tenant.Id}", tenant);
      }
      catch (UnauthorizedAccessException)
      {
        return Results.Unauthorized();
      }
      catch (Exception ex)
      {
        return Results.Problem(ex.Message);
      }
    })
    .WithName("CreateTenant")
    .WithOpenApi()
    .RequireAuthorization();
  }
}
