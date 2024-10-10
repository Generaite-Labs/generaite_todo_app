using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using ToDo.Domain.Events;
using ToDo.Domain.Interfaces;

namespace ToDo.Infrastructure.Services
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync(DomainEvent domainEvent)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handlers = _serviceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                MethodInfo? handleMethod = handlerType.GetMethod("HandleAsync");
                if (handleMethod != null)
                {
                    await (Task)handleMethod.Invoke(handler, new object[] { domainEvent })!;
                }
                else
                {
                    throw new InvalidOperationException($"HandleAsync method not found on handler type {handlerType.FullName}");
                }
            }
        }
    }
}