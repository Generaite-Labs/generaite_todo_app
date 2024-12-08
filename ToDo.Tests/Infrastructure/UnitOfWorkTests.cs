using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using ToDo.Domain.Events;
using ToDo.Domain.Interfaces;
using ToDo.Infrastructure;
using Xunit;

namespace ToDo.Tests.Infrastructure
{
  public class UnitOfWorkTests
  {
    private readonly Mock<ApplicationDbContext> _mockContext;
    private readonly Mock<IEventCollector> _mockEventCollector;
    private readonly Mock<IEventDispatcher> _mockEventDispatcher;
    private readonly Mock<DatabaseFacade> _mockDatabase;
    private readonly Mock<IDbContextTransaction> _mockTransaction;
    private readonly UnitOfWork _unitOfWork;

    public UnitOfWorkTests()
    {
      _mockContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
      _mockEventCollector = new Mock<IEventCollector>();
      _mockEventDispatcher = new Mock<IEventDispatcher>();
      _mockDatabase = new Mock<DatabaseFacade>(_mockContext.Object);
      _mockTransaction = new Mock<IDbContextTransaction>();

      // Setup database transaction
      _mockDatabase.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
          .ReturnsAsync(_mockTransaction.Object);
      _mockContext.Setup(c => c.Database).Returns(_mockDatabase.Object);

      _unitOfWork = new UnitOfWork(
          _mockContext.Object,
          _mockEventCollector.Object,
          _mockEventDispatcher.Object
      );
    }

    [Fact]
    public async Task SaveChangesAsync_SuccessfulOperation_CommitsTransactionAndReturnsTrue()
    {
      // Arrange
      var mockEvent = new Mock<DomainEvent>().Object;
      var events = new List<IDomainEvent> { mockEvent };

      _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
          .ReturnsAsync(1);
      _mockEventCollector.Setup(ec => ec.GetEvents())
          .Returns(events);

      // Act
      var result = await _unitOfWork.SaveChangesAsync();

      // Assert
      Assert.True(result);
      _mockTransaction.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
      _mockEventDispatcher.Verify(
          ed => ed.DispatchEventsAsync(It.IsAny<IEnumerable<DomainEvent>>(), It.IsAny<CancellationToken>()),
          Times.Once);
      _mockEventCollector.Verify(ec => ec.ClearEvents(), Times.Once);
    }

    [Fact]
    public async Task SaveChangesAsync_NoChanges_CommitsTransactionAndReturnsFalse()
    {
      // Arrange
      _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
          .ReturnsAsync(0);
      _mockEventCollector.Setup(ec => ec.GetEvents())
          .Returns(new List<IDomainEvent>());

      // Act
      var result = await _unitOfWork.SaveChangesAsync();

      // Assert
      Assert.False(result);
      _mockTransaction.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
      _mockEventDispatcher.Verify(
          ed => ed.DispatchEventsAsync(It.IsAny<IEnumerable<DomainEvent>>(), It.IsAny<CancellationToken>()),
          Times.Never);
      _mockEventCollector.Verify(ec => ec.ClearEvents(), Times.Never);
    }

    [Fact]
    public async Task SaveChangesAsync_Exception_RollsBackTransaction()
    {
      // Arrange
      _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
          .ThrowsAsync(new Exception("Test exception"));

      // Act & Assert
      await Assert.ThrowsAsync<Exception>(() => _unitOfWork.SaveChangesAsync());
      _mockTransaction.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
      _mockTransaction.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
  }
}
