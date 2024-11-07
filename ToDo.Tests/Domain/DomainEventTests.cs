using ToDo.Domain.Events;

namespace ToDo.Tests.Domain;

public class DomainEventTests
{
    private class TestEvent : DomainEvent
    {
        public TestEvent() : base() { }
    }

    [Fact]
    public void Constructor_ShouldCreateEventWithValidProperties()
    {
        // Arrange & Act
        var @event = new TestEvent();

        // Assert
        Assert.NotEqual(Guid.Empty, @event.Id);
        Assert.True(@event.OccurredOn <= DateTime.UtcNow);
        Assert.True(@event.OccurredOn > DateTime.UtcNow.AddSeconds(-1));
    }

    [Fact]
    public void OccurredOn_ShouldBeUtc()
    {
        // Arrange & Act
        var @event = new TestEvent();

        // Assert
        Assert.Equal(DateTimeKind.Utc, @event.OccurredOn.Kind);
    }

    [Fact]
    public void Id_ShouldBeUnique()
    {
        // Arrange & Act
        var event1 = new TestEvent();
        var event2 = new TestEvent();

        // Assert
        Assert.NotEqual(event1.Id, event2.Id);
    }

    [Fact]
    public void Constructor_ShouldSetPropertiesImmutably()
    {
        // Arrange & Act
        var @event = new TestEvent();

        // Assert
        // Verify properties are get-only through reflection
        var properties = typeof(DomainEvent).GetProperties();
        foreach (var property in properties)
        {
            Assert.False(property.CanWrite, $"Property {property.Name} should be read-only");
        }
    }

    [Fact]
    public void Event_CreatedInThePast_ShouldNotBeAllowed()
    {
        // Arrange & Act
        var @event = new TestEvent();

        // Assert
        Assert.True(@event.OccurredOn <= DateTime.UtcNow);
        Assert.True(@event.OccurredOn > DateTime.UtcNow.AddSeconds(-1));
    }
} 