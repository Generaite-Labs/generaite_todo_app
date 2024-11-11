using ToDo.Domain.Interfaces;

namespace ToDo.Domain.Events
{
    public class EventCollector : IEventCollector
    {
        private readonly List<IDomainEvent> _events = new();

        public void AddEvent(IDomainEvent domainEvent)
        {
            _events.Add(domainEvent);
        }

        public IReadOnlyCollection<IDomainEvent> GetEvents()
        {
            return _events.AsReadOnly();
        }

        public void ClearEvents()
        {
            _events.Clear();
        }
    }
} 