using ToDo.Domain.Events;

namespace ToDo.Domain.Interfaces {
    /// <summary>
    /// Defines a contract for dispatching domain events to their respective handlers.
    /// </summary>
    public interface IEventDispatcher
    {
        /// <summary>
        /// Dispatches a collection of domain events to their registered handlers.
        /// </summary>
        /// <param name="events">The collection of domain events to dispatch.</param>
        /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
        Task DispatchEventsAsync(IEnumerable<DomainEvent> events, CancellationToken cancellationToken = default);
    }
}