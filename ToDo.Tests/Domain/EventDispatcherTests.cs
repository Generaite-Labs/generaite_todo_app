using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using ToDo.Domain.Events;
using ToDo.Domain.Interfaces;

namespace ToDo.Tests.Domain
{
  public class EventDispatcherTests
  {
    private readonly Mock<ILogger<EventDispatcher>> _loggerMock;
    private readonly IServiceCollection _services;
    private readonly ServiceProvider _serviceProvider;

    public EventDispatcherTests()
    {
      _loggerMock = new Mock<ILogger<EventDispatcher>>();
      _services = new ServiceCollection();
      _serviceProvider = _services.BuildServiceProvider();
    }

    [Fact]
    public async Task DispatchEventsAsync_WithNoHandlers_LogsWarning()
    {
      // Arrange
      var dispatcher = new EventDispatcher(_serviceProvider, _loggerMock.Object);
      var testEvent = new TestEvent();

      // Act
      await dispatcher.DispatchEventsAsync(new[] { testEvent });

      // Assert
      _loggerMock.Verify(x => x.Log(
          LogLevel.Warning,
          It.IsAny<EventId>(),
          It.Is<It.IsAnyType>((v, t) => true),
          It.IsAny<Exception>(),
          It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
          Times.Once);
    }

    [Fact]
    public async Task DispatchEventsAsync_WithHandler_InvokesHandler()
    {
      // Arrange
      var handlerMock = new Mock<IDomainEventHandler<TestEvent>>();
      _services.AddScoped(_ => handlerMock.Object);
      var serviceProvider = _services.BuildServiceProvider();

      var dispatcher = new EventDispatcher(serviceProvider, _loggerMock.Object);
      var testEvent = new TestEvent();

      // Act
      await dispatcher.DispatchEventsAsync(new[] { testEvent });

      // Assert
      handlerMock.Verify(x => x.HandleAsync(testEvent, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DispatchEventsAsync_WhenHandlerThrows_PropagatesException()
    {
      // Arrange
      var handlerMock = new Mock<IDomainEventHandler<TestEvent>>();
      handlerMock.Setup(x => x.HandleAsync(It.IsAny<TestEvent>(), It.IsAny<CancellationToken>()))
          .ThrowsAsync(new Exception("Test exception"));

      _services.AddScoped(_ => handlerMock.Object);
      var serviceProvider = _services.BuildServiceProvider();

      var dispatcher = new EventDispatcher(serviceProvider, _loggerMock.Object);
      var testEvent = new TestEvent();

      // Act & Assert
      await Assert.ThrowsAsync<Exception>(() =>
          dispatcher.DispatchEventsAsync(new[] { testEvent }));
    }
  }

  public class TestEvent : DomainEvent { }
}
