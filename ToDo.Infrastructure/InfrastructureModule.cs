using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToDo.Domain.Entities;

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

    services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<TodoDbContext>()
        .AddDefaultTokenProviders();

    services.Configure<IdentityOptions>(options =>
    {
      // Password settings
      options.Password.RequireDigit = true;
      options.Password.RequireLowercase = true;
      options.Password.RequireNonAlphanumeric = true;
      options.Password.RequireUppercase = true;
      options.Password.RequiredLength = 6;
      options.Password.RequiredUniqueChars = 1;

      // Lockout settings
      options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
      options.Lockout.MaxFailedAccessAttempts = 5;
      options.Lockout.AllowedForNewUsers = true;

      // User settings
      options.User.AllowedUserNameCharacters =
                  "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
      options.User.RequireUniqueEmail = false;
    });

    // Register other infrastructure services...

    return services;
  }
}