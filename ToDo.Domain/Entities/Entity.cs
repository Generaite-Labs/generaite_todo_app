using System.Collections.Generic;
using ToDo.Domain.Interfaces;

namespace ToDo.Domain.Entities;

/// <summary>
/// Base class for all entities in the domain model
/// </summary>
/// <typeparam name="TId">The type of the entity's identifier</typeparam>
public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : notnull
{
    /// <summary>
    /// Gets the unique identifier of the entity
    /// </summary>
    public TId Id { get; protected set; }

    protected Entity(TId id)
    {
        Id = id;
    }

    #region Equality Members

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals(obj as Entity<TId>);
    }

    public bool Equals(Entity<TId>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<TId>.Default.GetHashCode(Id);
    }

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return $"{GetType().Name} [Id={Id}]";
    }

    #endregion
}

/// <summary>
/// Non-generic convenience base class for entities with int IDs
/// </summary>
public abstract class Entity : Entity<int>
{
    protected Entity(int id) : base(id)
    {
    }
}
