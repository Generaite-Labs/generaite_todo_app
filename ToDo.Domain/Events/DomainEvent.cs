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
        /// The version of the aggregate when this event was created
        /// </summary>
        public long Version { get; }

        /// <summary>
        /// The ID of the aggregate that raised this event
        /// </summary>
        public Guid AggregateId { get; }

        /// <summary>
        /// The type name of the aggregate that raised this event
        /// </summary>
        public string AggregateType { get; }

        /// <summary>
        /// Creates a new domain event
        /// </summary>
        protected DomainEvent(Guid aggregateId, string aggregateType, long version)
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            AggregateId = aggregateId;
            AggregateType = aggregateType;
            Version = version;
        }

        // Parameterless constructor for testing/serialization
        protected DomainEvent() : this(Guid.Empty, string.Empty, 0)
        {
        }
    }
}
