using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ToDo.Core.Configuration;
using ToDo.Domain.Interfaces;
using ToDo.Infrastructure.Repositories;
using ToDo.Application.Services;
using ToDo.Application.Validators;
using ToDo.Application.Interfaces;
using FluentValidation;
using ToDo.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using ToDo.Domain.Entities;
using ToDo.Domain.Events;

namespace ToDo.Infrastructure;

public static class InfrastructureModule
{
  public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
  {
    services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.Database));
    services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
    {
      var databaseOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
      options.UseNpgsql(databaseOptions.ConnectionString);
    });

    // Add Identity configuration
    services.AddIdentityCore<ApplicationUser>(options =>
    {
      options.SignIn.RequireConfirmedAccount = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddApiEndpoints()
    .AddDefaultTokenProviders();

    // Add SignInManager explicitly
    services.AddScoped<SignInManager<ApplicationUser>>();

    // Repositories
    services.AddScoped<ITodoItemRepository, TodoItemRepository>();
    services.AddScoped<ITenantRepository, TenantRepository>();

    // Services
    services.AddScoped<ITodoItemService, TodoItemService>();
    services.AddScoped<ITenantService, TenantService>();

    // Validators
    services.AddValidatorsFromAssemblyContaining<CreateTodoItemDtoValidator>();

    // Register UnitOfWork
    services.AddScoped<IUnitOfWork, UnitOfWork>();

    // Register EventCollector
    services.AddScoped<IEventCollector, EventCollector>();
    services.AddScoped<IEventDispatcher, EventDispatcher>();

    // Register AutoMapper
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    // Register CurrentUserService
    services.AddScoped<ICurrentUserService, CurrentUserService>();

    // Add HttpContextAccessor
    services.AddHttpContextAccessor();

    return services;
  }
}
