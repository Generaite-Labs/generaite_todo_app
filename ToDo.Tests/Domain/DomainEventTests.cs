using ToDo.Domain.Events;

namespace ToDo.Tests.Domain;

public class DomainEventTests
{
    private class TestEvent : DomainEvent
    {
        public TestEvent(string eventType, string userId) : base(eventType, userId) { }
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateEvent()
    {
        // Arrange & Act
        var @event = new TestEvent("TestEvent", "user123");

        // Assert
        Assert.NotEqual(Guid.Empty, @event.Id);
        Assert.Equal("TestEvent", @event.EventType);
        Assert.Equal("user123", @event.UserId);
        Assert.True(@event.OccurredOn <= DateTime.UtcNow);
        Assert.True(@event.OccurredOn > DateTime.UtcNow.AddSeconds(-1));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_WithInvalidEventType_ShouldThrowArgumentNullException(string eventType)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => 
            new TestEvent(eventType, "validUserId"));
        Assert.Equal("eventType", ex.ParamName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_WithInvalidUserId_ShouldThrowArgumentNullException(string userId)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => 
            new TestEvent("validEventType", userId));
        Assert.Equal("userId", ex.ParamName);
    }

    [Fact]
    public void OccurredOn_ShouldBeUtc()
    {
        // Arrange & Act
        var @event = new TestEvent("TestEvent", "user123");

        // Assert
        Assert.Equal(DateTimeKind.Utc, @event.OccurredOn.Kind);
    }

    [Fact]
    public void Id_ShouldBeUnique()
    {
        // Arrange & Act
        var event1 = new TestEvent("TestEvent", "user123");
        var event2 = new TestEvent("TestEvent", "user123");

        // Assert
        Assert.NotEqual(event1.Id, event2.Id);
    }

    [Fact]
    public void Constructor_ShouldSetPropertiesImmutably()
    {
        // Arrange
        var eventType = "TestEvent";
        var userId = "user123";

        // Act
        var @event = new TestEvent(eventType, userId);

        // Assert
        Assert.Equal(eventType, @event.EventType);
        Assert.Equal(userId, @event.UserId);
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
        var @event = new TestEvent("TestEvent", "user123");

        // Assert
        Assert.True(@event.OccurredOn <= DateTime.UtcNow);
        Assert.True(@event.OccurredOn > DateTime.UtcNow.AddSeconds(-1));
    }
} 