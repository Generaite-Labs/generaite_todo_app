using System.Collections.Generic;

namespace ToDo.Domain.Interfaces
{
  public interface IEventCollector
  {
    void AddEvent(IDomainEvent domainEvent);
    IReadOnlyCollection<IDomainEvent> GetEvents();
    void ClearEvents();
  }
}
