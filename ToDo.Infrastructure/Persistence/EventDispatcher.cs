using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Reflection;
using ToDo.Domain.Interfaces;

namespace ToDo.Domain.Events
{
  /// <summary>
  /// Implements the event dispatcher pattern for domain events, handling the routing of events to their appropriate handlers.
  /// This implementation uses reflection to dynamically find and invoke handlers, with results cached for performance.
  /// </summary>
  /// <remarks>
  /// Key features:
  /// - Supports multiple handlers per event type
  /// - Uses fast-fail approach (throws on first error)
  /// - Caches method information for better performance
  /// - Provides detailed logging for debugging and monitoring
  /// - Supports cancellation
  /// </remarks>
  public class EventDispatcher : IEventDispatcher
  {
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventDispatcher> _logger;

    /// <summary>
    /// Cache for storing reflected method information to improve performance.
    /// Key: Handler type, Value: Cached MethodInfo for HandleAsync
    /// </summary>
    private static readonly ConcurrentDictionary<Type, MethodInfo> _methodCache = new();

    /// <summary>
    /// Initializes a new instance of the EventDispatcher.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve event handlers.</param>
    /// <param name="logger">Logger for diagnostic information.</param>
    /// <exception cref="ArgumentNullException">Thrown if serviceProvider or logger is null.</exception>
    public EventDispatcher(IServiceProvider serviceProvider, ILogger<EventDispatcher> logger)
    {
      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Dispatches a collection of domain events to their registered handlers.
    /// </summary>
    /// <param name="events">The collection of domain events to dispatch.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
    /// <exception cref="ArgumentNullException">Thrown if events collection is null.</exception>
    /// <exception cref="Exception">Propagates any exceptions thrown by event handlers.</exception>
    public async Task DispatchEventsAsync(IEnumerable<DomainEvent> events, CancellationToken cancellationToken = default)
    {
      if (events == null) throw new ArgumentNullException(nameof(events));

      foreach (var @event in events)
      {
        _logger.LogDebug("Dispatching event of type {EventType}", @event.GetType().Name);

        try
        {
          // Create the generic handler type for this specific event
          var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(@event.GetType());

          // Get all registered handlers for this event type
          var handlers = _serviceProvider.GetServices(handlerType).Where(h => h != null);

          if (!handlers.Any())
          {
            _logger.LogWarning("No handlers found for event type {EventType}", @event.GetType().Name);
            continue;
          }

          // Invoke each handler
          foreach (var handler in handlers)
          {
            if (handler != null)
            {
              await InvokeHandlerAsync(handler, handlerType, @event, cancellationToken);
            }
          }
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Failed to dispatch event {EventType}. Error: {ErrorMessage}",
              @event.GetType().Name, ex.Message);
          throw; // Fast-fail approach
        }
      }
    }

    /// <summary>
    /// Invokes a single event handler using reflection.
    /// </summary>
    /// <param name="handler">The handler instance to invoke.</param>
    /// <param name="handlerType">The type of the handler.</param>
    /// <param name="event">The event to pass to the handler.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <exception cref="ArgumentNullException">Thrown if handler is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the handler method is not found or returns null.</exception>
    private async Task InvokeHandlerAsync(object handler, Type handlerType, DomainEvent @event,
        CancellationToken cancellationToken)
    {
      if (handler == null) throw new ArgumentNullException(nameof(handler));

      // Get or create the cached method info for this handler type
      var method = _methodCache.GetOrAdd(handlerType, type =>
      {
        // Find all methods named HandleAsync
        var methods = type.GetMethods()
                  .Where(m => m.Name == "HandleAsync")
                  .ToList();

        // Find the one with the correct parameter types
        var methodInfo = methods.FirstOrDefault(m =>
              {
                var parameters = m.GetParameters();
                return parameters.Length == 2 &&
                             parameters[0].ParameterType == type.GetGenericArguments()[0] &&
                             parameters[1].ParameterType == typeof(CancellationToken);
              });

        return methodInfo ?? throw new InvalidOperationException(
                  $"Method HandleAsync with expected signature not found on {type.Name}");
      });

      try
      {
        // Invoke the handler method
        var result = method.Invoke(handler, new object[] { @event, cancellationToken });
        if (result == null)
        {
          throw new InvalidOperationException(
              $"Handler {handlerType.Name} returned null from HandleAsync method");
        }
        await (Task)result;
      }
      catch (TargetInvocationException ex)
      {
        // Unwrap the inner exception for clearer error reporting
        throw ex.InnerException ?? ex;
      }
    }
  }
}
