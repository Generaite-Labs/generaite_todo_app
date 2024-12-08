using System.Globalization;
using ToDo.Infrastructure.Context;

namespace ToDo.Tests.Infrastructure
{
  public class ApplicationContextTests
  {
    [Fact]
    public void Constructor_WithValidUserId_ShouldCreateContext()
    {
      // Arrange
      var userId = "test-user";

      // Act
      var context = new ApplicationContext(userId);

      // Assert
      Assert.Equal(userId, context.UserId);
      Assert.NotNull(context.CorrelationId);
      Assert.True(context.Timestamp <= DateTime.UtcNow);
      Assert.True(context.Timestamp > DateTime.UtcNow.AddSeconds(-1));
      Assert.Equal(CultureInfo.CurrentCulture, context.Culture);
    }

    [Fact]
    public void Constructor_WithNullUserId_ShouldThrowArgumentNullException()
    {
      // Act & Assert
      Assert.Throws<ArgumentNullException>(() => new ApplicationContext(null!));
    }

    [Fact]
    public void Constructor_WithCustomValues_ShouldRespectAllParameters()
    {
      // Arrange
      var userId = "test-user";
      var correlationId = "test-correlation";
      var culture = new CultureInfo("es-ES");

      // Act
      var context = new ApplicationContext(userId, correlationId, culture);

      // Assert
      Assert.Equal(userId, context.UserId);
      Assert.Equal(correlationId, context.CorrelationId);
      Assert.Equal(culture, context.Culture);
    }

    [Fact]
    public void Constructor_WithoutCorrelationId_ShouldGenerateNewGuid()
    {
      // Arrange & Act
      var context = new ApplicationContext("test-user");

      // Assert
      Assert.NotNull(context.CorrelationId);
      Assert.True(Guid.TryParse(context.CorrelationId, out _));
    }
  }
}
