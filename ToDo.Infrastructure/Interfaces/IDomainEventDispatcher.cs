using ToDo.Domain.Events;
using ToDo.Domain.Interfaces;

namespace ToDo.Infrastructure.Interfaces {
    public interface IDomainEventDispatcher
    {
        Task DispatchEventAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
        Task DispatchEventsAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
    }
}