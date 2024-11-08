using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Events;
using ToDo.Domain.Interfaces;

namespace ToDo.Infrastructure
{
    /// <summary>
    /// Implementation of the Unit of Work pattern that coordinates database operations
    /// and domain event dispatching within a transaction boundary.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TodoDbContext _context;
        private readonly IEventCollector _eventCollector;
        private readonly IEventDispatcher _eventDispatcher;
        
        public UnitOfWork(
            TodoDbContext context,
            IEventCollector eventCollector,
            IEventDispatcher eventDispatcher)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _eventCollector = eventCollector ?? throw new ArgumentNullException(nameof(eventCollector));
            _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
        }
        
        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // 1. Save all changes to the database
                var result = await _context.SaveChangesAsync(cancellationToken) > 0;
                
                // 2. Get collected events and cast them to DomainEvent, filtering out nulls
                var events = _eventCollector.GetEvents()
                    .OfType<DomainEvent>() // This safely filters and casts at the same time
                    .ToList();
                
                if (events.Any())
                {
                    // 3. Dispatch all collected events
                    await _eventDispatcher.DispatchEventsAsync(events, cancellationToken);
                    
                    // 4. Clear the event collector after successful dispatch
                    _eventCollector.ClearEvents();
                }
                
                // 5. Commit transaction
                await transaction.CommitAsync(cancellationToken);
                return result;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
} 