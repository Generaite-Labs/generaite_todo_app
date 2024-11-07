using ToDo.Domain.Events;
using ToDo.Domain.Interfaces;
using Xunit;
using Moq;

namespace ToDo.Tests.Domain
{
    public class EventCollectorTests
    {
        private readonly EventCollector _eventCollector;

        public EventCollectorTests()
        {
            _eventCollector = new EventCollector();
        }

        [Fact]
        public void AddEvent_ShouldAddEventToCollection()
        {
            // Arrange
            var mockEvent = new Mock<IDomainEvent>();

            // Act
            _eventCollector.AddEvent(mockEvent.Object);

            // Assert
            Assert.Single(_eventCollector.GetEvents());
            Assert.Contains(mockEvent.Object, _eventCollector.GetEvents());
        }

        [Fact]
        public void GetEvents_ShouldReturnEventsInOrderOfAddition()
        {
            // Arrange
            var mockEvent1 = new Mock<IDomainEvent>();
            var mockEvent2 = new Mock<IDomainEvent>();

            // Act
            _eventCollector.AddEvent(mockEvent1.Object);
            _eventCollector.AddEvent(mockEvent2.Object);
            var events = _eventCollector.GetEvents();

            // Assert
            Assert.Equal(2, events.Count);
            Assert.Equal(mockEvent1.Object, events.First());
            Assert.Equal(mockEvent2.Object, events.Last());
        }

        [Fact]
        public void ClearEvents_ShouldRemoveAllEvents()
        {
            // Arrange
            var mockEvent = new Mock<IDomainEvent>();
            _eventCollector.AddEvent(mockEvent.Object);

            // Act
            _eventCollector.ClearEvents();

            // Assert
            Assert.Empty(_eventCollector.GetEvents());
        }
    }
} 