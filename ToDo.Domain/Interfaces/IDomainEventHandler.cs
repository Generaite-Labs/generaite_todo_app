using ToDo.Domain.Events;

namespace ToDo.Domain.Interfaces
{
    public interface IDomainEventHandler<TEvent> where TEvent : DomainEvent
    {
        Task HandleAsync(TEvent domainEvent);
    }
}