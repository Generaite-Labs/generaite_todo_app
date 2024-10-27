using System.ComponentModel.DataAnnotations.Schema;
using ToDo.Domain.Interfaces;

namespace ToDo.Domain.Events
{
    [NotMapped]
    public abstract class DomainEvent : IDomainEvent
    {
        public Guid Id { get; }
        public DateTime OccurredOn { get; }
        public string EventType { get; }
        public string UserId { get; }

        protected DomainEvent(string eventType, string userId)
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            EventType = eventType;
            UserId = userId;
        }
    }
}
