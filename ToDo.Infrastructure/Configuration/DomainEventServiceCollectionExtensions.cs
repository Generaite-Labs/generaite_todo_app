using Microsoft.Extensions.DependencyInjection;
using ToDo.Domain.Events;
using ToDo.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ToDo.Infrastructure.Configuration
{
    public static class DomainEventServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the domain event system services to the dependency injection container.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddDomainEventSystem(this IServiceCollection services)
        {
            // Validate that required services are registered
            if (!services.Any(s => s.ServiceType == typeof(DbContext)))
            {
                throw new InvalidOperationException(
                    "DbContext must be registered before adding the domain event system.");
            }

            // Core event system components
            services.AddScoped<IEventCollector, EventCollector>();
            services.AddScoped<IEventDispatcher, EventDispatcher>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Scan and register all event handlers
            var assemblyWithHandlers = typeof(EventDispatcher).Assembly;
            var handlerTypes = assemblyWithHandlers.GetTypes()
                .Where(t => t.GetInterfaces()
                    .Any(i => i.IsGenericType && 
                             i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)));

            foreach (var handlerType in handlerTypes)
            {
                var handlerInterface = handlerType.GetInterfaces()
                    .First(i => i.IsGenericType && 
                               i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>));
                services.AddScoped(handlerInterface, handlerType);
            }

            return services;
        }
    }
} 