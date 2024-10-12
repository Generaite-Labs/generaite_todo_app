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

        // Repositories
        services.AddScoped<ITodoItemRepository, TodoItemRepository>();

        // Services
        services.AddScoped<ITodoItemService, TodoItemService>();

        // Validators
        services.AddValidatorsFromAssemblyContaining<CreateTodoItemDtoValidator>();

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        return services;
    }
}