using ToDo.Domain.Entities;
using ToDo.Domain.Interfaces;

namespace ToDo.Application.Services
{
    public class DomainEventService : IDomainEventService
    {
        private readonly IDomainEventDispatcher _dispatcher;

        public DomainEventService(IDomainEventDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public async Task PublishEventsAsync(IEnumerable<TodoItem> entities)
        {
            foreach (var entity in entities)
            {
                var events = entity.DomainEvents.ToArray();
                entity.ClearDomainEvents();
                foreach (var domainEvent in events)
                {
                    await _dispatcher.DispatchAsync(domainEvent);
                }
            }
        }
    }
}