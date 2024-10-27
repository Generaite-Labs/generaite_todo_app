using ToDo.Domain.Events;

namespace ToDo.Domain.Interfaces
{
    /// <summary>
    /// Interface for entities that can raise domain events
    /// </summary>
    public interface IHasDomainEvents 
    {
        /// <summary>
        /// Gets the collection of domain events that have been raised but not yet dispatched
        /// </summary>
        IReadOnlyCollection<DomainEvent> DomainEvents { get; }

        /// <summary>
        /// Adds a domain event to the entity's collection of pending events
        /// </summary>
        /// <param name="evt">The domain event to add</param>
        void AddDomainEvent(DomainEvent evt);

        /// <summary>
        /// Clears all pending domain events from the entity
        /// </summary>
        void ClearDomainEvents();
    }
}
