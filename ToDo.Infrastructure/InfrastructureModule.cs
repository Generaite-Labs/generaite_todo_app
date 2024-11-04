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

namespace ToDo.Infrastructure;

public static class InfrastructureModule
{
  public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
  {
    // Add this line to configure DatabaseOptions
    services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.Database));

    services.AddDbContext<TodoDbContext>((serviceProvider, options) =>
    {
      var databaseOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
      options.UseNpgsql(databaseOptions.ConnectionString);
    });

    // Add Identity configuration
    services.AddIdentityCore<ApplicationUser>(options => {
      options.SignIn.RequireConfirmedAccount = true;
    })
    .AddEntityFrameworkStores<TodoDbContext>()
    .AddApiEndpoints()
    .AddDefaultTokenProviders();

    // Add SignInManager explicitly
    services.AddScoped<SignInManager<ApplicationUser>>();

    // Repositories
    services.AddScoped<ITodoItemRepository, TodoItemRepository>();

    // Services
    services.AddScoped<ITodoItemService, TodoItemService>();

    // Validators
    services.AddValidatorsFromAssemblyContaining<CreateTodoItemDtoValidator>();

    return services;
  }
}
