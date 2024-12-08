using ToDo.Domain.Events;

namespace ToDo.Domain.Interfaces
{
  public interface IDomainEventHandler<in TEvent> where TEvent : DomainEvent
  {
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
  }
}
