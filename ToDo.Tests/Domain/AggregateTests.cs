using ToDo.Domain.Entities;
using ToDo.Domain.Interfaces;
using Xunit;

namespace ToDo.Tests.Domain;

public class AggregateTests
{
    // Test classes
    private class TestAggregateRoot : AggregateRoot<int>
    {
        public TestAggregateRoot(int id) : base(id) { }
        
        public void AddTestChild(TestChildEntity child)
        {
            EnsureChildBelongsToAggregate(child);
        }
    }

    private class TestChildEntity : AggregateEntity<int, int>
    {
        public TestChildEntity(int id, int aggregateRootId) : base(id, aggregateRootId) { }
    }

    private record TestDomainEvent : IDomainEvent
    {
        public Guid Id => throw new NotImplementedException();

        public DateTime OccurredOn => throw new NotImplementedException();

        public string EventType => throw new NotImplementedException();

        public string UserId => throw new NotImplementedException();
        public long Version => throw new NotImplementedException();
        public Guid AggregateId => throw new NotImplementedException();
        public string AggregateType => throw new NotImplementedException();
    }


    [Fact]
    public void AggregateRoot_Version_StartsAtZero()
    {
        // Arrange & Act
        var aggregate = new TestAggregateRoot(1);

        // Assert
        Assert.Equal(0, aggregate.Version);
    }

    [Fact]
    public void AggregateRoot_RaiseDomainEvent_IncrementsVersion()
    {
        // Arrange
        var aggregate = new TestAggregateRoot(1);
        var initialVersion = aggregate.Version;

        // Act
        aggregate.RaiseDomainEvent(new TestDomainEvent());

        // Assert
        Assert.Equal(initialVersion + 1, aggregate.Version);
    }

    [Fact]
    public void AggregateRoot_EnsureChildBelongsToAggregate_AcceptsValidChild()
    {
        // Arrange
        var aggregate = new TestAggregateRoot(1);
        var child = new TestChildEntity(2, aggregate.Id);

        // Act & Assert
        var exception = Record.Exception(() => aggregate.AddTestChild(child));
        Assert.Null(exception);
    }

    [Fact]
    public void AggregateRoot_EnsureChildBelongsToAggregate_ThrowsForInvalidChild()
    {
        // Arrange
        var aggregate = new TestAggregateRoot(1);
        var child = new TestChildEntity(2, 999); // Different aggregate root ID

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => aggregate.AddTestChild(child));
    }

    [Fact]
    public void AggregateEntity_RaiseDomainEvent_WorksThroughCorrectAggregateRoot()
    {
        // Arrange
        var aggregate = new TestAggregateRoot(1);
        var child = new TestChildEntity(2, aggregate.Id);
        var initialVersion = aggregate.Version;

        // Act
        child.RaiseDomainEvent(aggregate, new TestDomainEvent());

        // Assert
        Assert.Equal(initialVersion + 1, aggregate.Version);
    }

    [Fact]
    public void AggregateEntity_RaiseDomainEvent_ThrowsForWrongAggregateRoot()
    {
        // Arrange
        var aggregate = new TestAggregateRoot(999);
        var child = new TestChildEntity(2, 1); // Belongs to different aggregate

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => child.RaiseDomainEvent(aggregate, new TestDomainEvent()));
    }
} 