using ToDo.Domain.Interfaces;

namespace ToDo.Domain.Entities;

/// <summary>
/// Base class for entities that belong to an aggregate root
/// </summary>
/// <typeparam name="TId">The type of this entity's identifier</typeparam>
/// <typeparam name="TAggregateId">The type of the aggregate root's identifier</typeparam>
public abstract class AggregateEntity<TId, TAggregateId> : Entity<TId>, IEntityBelongsToAggregate
    where TId : notnull
    where TAggregateId : notnull
{
  private AggregateRoot<TAggregateId>? _aggregateRoot;

  /// <summary>
  /// Gets the ID of the aggregate root this entity belongs to
  /// </summary>
  public TAggregateId AggregateRootId { get; }

  object? IEntityBelongsToAggregate.AggregateRootId => AggregateRootId;

  protected AggregateEntity(TId id, TAggregateId aggregateRootId) : base(id)
  {
    AggregateRootId = aggregateRootId;
  }

  /// <summary>
  /// Sets the aggregate root reference for this entity
  /// </summary>
  internal void SetAggregateRoot(AggregateRoot<TAggregateId> aggregateRoot)
  {
    VerifyBelongsToAggregate(aggregateRoot);
    _aggregateRoot = aggregateRoot;
  }

  /// <summary>
  /// Raises a domain event through the aggregate root
  /// </summary>
  /// <param name="domainEvent">The domain event to raise</param>
  internal protected void RaiseDomainEvent(IDomainEvent domainEvent)
  {
    if (_aggregateRoot == null)
    {
      throw new InvalidOperationException(
          $"Cannot raise domain event: Entity {GetType().Name} is not associated with an aggregate root");
    }
    _aggregateRoot.RaiseDomainEvent(domainEvent);
  }

  /// <summary>
  /// Verifies that this entity belongs to the specified aggregate root
  /// </summary>
  /// <param name="aggregateRoot">The aggregate root to verify against</param>
  /// <typeparam name="TAggregateRoot">The type of the aggregate root</typeparam>
  public void VerifyBelongsToAggregate<TAggregateRoot>(TAggregateRoot aggregateRoot)
      where TAggregateRoot : AggregateRoot<TAggregateId>
  {
    if (!AggregateRootId.Equals(aggregateRoot.Id))
    {
      throw new InvalidOperationException(
          $"Entity {GetType().Name} with ID {Id} belongs to aggregate root " +
          $"with ID {AggregateRootId}, not {aggregateRoot.Id}");
    }
  }
}
