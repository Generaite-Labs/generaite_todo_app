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
        /// Type of the event for classification
        /// </summary>
        public string EventType { get; }

        /// <summary>
        /// ID of the user who triggered the event
        /// </summary>
        public string UserId { get; }

        /// <summary>
        /// Creates a new domain event
        /// </summary>
        /// <param name="eventType">Type of the event</param>
        /// <param name="userId">ID of the user triggering the event</param>
        /// <exception cref="ArgumentNullException">Thrown when eventType or userId is null</exception>
        protected DomainEvent(string eventType, string userId)
        {
            if (string.IsNullOrWhiteSpace(eventType))
                throw new ArgumentNullException(nameof(eventType));
            
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            EventType = eventType;
            UserId = userId;
        }
    }
}
