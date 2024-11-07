using System.ComponentModel.DataAnnotations.Schema;
using ToDo.Domain.Interfaces;

namespace ToDo.Domain.Events
{
    /// <summary>
    /// Base class for all domain events in the system
    /// </summary>
    [NotMapped]
    public abstract class DomainEvent : IDomainEvent
    {
        /// <summary>
        /// Unique identifier for the event
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// UTC timestamp when the event occurred
        /// </summary>
        public DateTime OccurredOn { get; }

        /// <summary>
        /// Creates a new domain event
        /// </summary>
        protected DomainEvent()
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
        }
    }
}
