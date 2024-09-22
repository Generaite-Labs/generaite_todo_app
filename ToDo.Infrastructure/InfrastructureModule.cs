using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ToDo.Infrastructure;

public static class InfrastructureModule
{
  public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
  {
    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    services.Configure<DatabaseConfig>(options =>
        options.ConnectionString = connectionString);

    services.AddDbContext<TodoDbContext>(options =>
        options.UseNpgsql(connectionString));

    // Register other infrastructure services...

    return services;
  }
}