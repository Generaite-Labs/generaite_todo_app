using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ToDo.Domain.Entities;
using ToDo.Core.Configuration;
using ToDo.Domain.Interfaces;
using ToDo.Infrastructure.Repositories;

namespace ToDo.Infrastructure;

public static class InfrastructureModule
{
  public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddDbContext<TodoDbContext>((serviceProvider, options) =>
    {
      var databaseOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
      options.UseNpgsql(databaseOptions.ConnectionString);
    });

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


    services.AddScoped<ITodoItemRepository, TodoItemRepository>();
    services.AddScoped<ITodoItemListRepository, TodoItemListRepository>();

    // Register other infrastructure services...

    return services;
  }
}