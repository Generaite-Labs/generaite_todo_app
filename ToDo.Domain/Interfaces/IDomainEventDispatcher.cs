using ToDo.Domain.Events;

namespace ToDo.Domain.Interfaces
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(DomainEvent domainEvent);
    }
}