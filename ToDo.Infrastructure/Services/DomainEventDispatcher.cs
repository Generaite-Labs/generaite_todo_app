using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using ToDo.Domain.Events;
using ToDo.Domain.Interfaces;
using ToDo.Infrastructure.Interfaces;
namespace ToDo.Infrastructure.Services
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DomainEventDispatcher> _logger;

        public DomainEventDispatcher(IServiceProvider serviceProvider, ILogger<DomainEventDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task DispatchEventAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handlers = _serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                try
                {
                    MethodInfo? handleMethod = handlerType.GetMethod("HandleAsync");
                    if (handleMethod != null)
                    {
                        await (Task)handleMethod.Invoke(handler, new object[] { domainEvent, cancellationToken })!;
                    }
                    else
                    {
                        throw new InvalidOperationException($"HandleAsync method not found on handler type {handlerType.FullName}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling domain event {EventType} with handler {HandlerType}", domainEvent.GetType().Name, handler.GetType().Name);
                    // Decide whether to rethrow the exception or continue with other handlers
                }
            }
        }
        public async Task DispatchEventsAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default) 
        {
            foreach (var domainEvent in domainEvents)
            {
                await DispatchEventAsync(domainEvent, cancellationToken);
            }
        }
    }
}
