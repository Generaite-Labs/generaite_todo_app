using ToDo.Domain.Interfaces;

namespace ToDo.Domain.Entities;

/// <summary>
/// Marker interface for aggregate roots
/// </summary>
public interface IAggregateRoot { }

/// <summary>
/// Base class for aggregate roots in the domain model
/// </summary>
/// <typeparam name="TId">The type of the aggregate's identifier</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
    where TId : notnull
{
    private int _version;
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Gets the version number of this aggregate root, used for optimistic concurrency
    /// </summary>
    public int Version => _version;

    /// <summary>
    /// Gets the domain events for this aggregate root
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected AggregateRoot(TId id) : base(id)
    {
        _version = 0;
    }

    /// <summary>
    /// Increments the version of this aggregate root
    /// </summary>
    internal void IncrementVersion()
    {
        _version++;
    }

    /// <summary>
    /// Raises a domain event for this aggregate root or its child entities
    /// </summary>
    /// <param name="domainEvent">The domain event to raise</param>
    public void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
        IncrementVersion();
    }

    /// <summary>
    /// Clears the domain events for this aggregate root
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Ensures that a child entity belongs to this aggregate root
    /// </summary>
    /// <param name="entity">The entity to check</param>
    /// <exception cref="InvalidOperationException">Thrown when the entity doesn't belong to this aggregate</exception>
    protected void EnsureChildBelongsToAggregate<TChildId>(AggregateEntity<TChildId, TId> entity)
        where TChildId : notnull
    {
        if (!entity.AggregateRootId.Equals(Id))
        {
            throw new InvalidOperationException(
                $"Entity {entity.GetType().Name} does not belong to aggregate {GetType().Name} with ID {Id}");
        }
    }

    /// <summary>
    /// Associates a child entity with this aggregate root
    /// </summary>
    /// <param name="entity">The entity to associate</param>
    protected void AssociateChild<TChildId>(AggregateEntity<TChildId, TId> entity)
        where TChildId : notnull
    {
        EnsureChildBelongsToAggregate(entity);
        entity.SetAggregateRoot(this);
    }
}

/// <summary>
/// Non-generic convenience base class for aggregate roots with int IDs
/// </summary>
public abstract class AggregateRoot : AggregateRoot<int>
{
    protected AggregateRoot(int id) : base(id)
    {
    }
} 