namespace ToDo.Domain.Interfaces;

/// <summary>
/// Interface for entities that belong to an aggregate root
/// </summary>
public interface IEntityBelongsToAggregate
{
    /// <summary>
    /// Gets the ID of the aggregate root this entity belongs to
    /// </summary>
    object? AggregateRootId { get; }
} 