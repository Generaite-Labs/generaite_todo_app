
using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Entities;
using ToDo.Domain.Common;
using ToDo.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace ToDo.Infrastructure.Tests
{
  public class TodoItemRepositoryTests : IDisposable
  {
    private readonly TodoDbContext _context;
    private readonly TodoItemRepository _repository;
    private readonly Mock<ILogger<TodoItemRepository>> _mockLogger;

    public TodoItemRepositoryTests()
    {
      var options = new DbContextOptionsBuilder<TodoDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;

      _context = new TodoDbContext(options);
      _mockLogger = new Mock<ILogger<TodoItemRepository>>();
      _repository = new TodoItemRepository(_context, _mockLogger.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectTodoItem()
    {
      // Arrange
      var todoItem = new TodoItem { Title = "Test Item", UserId = "user1" };
      _context.TodoItems.Add(todoItem);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.GetByIdAsync(todoItem.Id);

      // Assert
      Assert.NotNull(result);
      Assert.Equal(todoItem.Id, result.Id);
      Assert.Equal(todoItem.Title, result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNullForNonexistentId()
    {
      // Act
      var result = await _repository.GetByIdAsync(-1);

      // Assert
      Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllTodoItems()
    {
      // Arrange
      var todoItem1 = new TodoItem { Title = "Test Item 1", UserId = "user1" };
      var todoItem2 = new TodoItem { Title = "Test Item 2", UserId = "user1" };
      _context.TodoItems.AddRange(todoItem1, todoItem2);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.GetAllAsync();

      // Assert
      Assert.Equal(2, result.Count());
      Assert.Contains(result, t => t.Title == "Test Item 1");
      Assert.Contains(result, t => t.Title == "Test Item 2");
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsCorrectTodoItems()
    {
      // Arrange
      var todoItem1 = new TodoItem { Title = "Test Item 1", UserId = "user1" };
      var todoItem2 = new TodoItem { Title = "Test Item 2", UserId = "user1" };
      var todoItem3 = new TodoItem { Title = "Test Item 3", UserId = "user2" };
      _context.TodoItems.AddRange(todoItem1, todoItem2, todoItem3);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.GetByUserIdAsync("user1");

      // Assert
      Assert.Equal(2, result.Count());
      Assert.Contains(result, t => t.Title == "Test Item 1");
      Assert.Contains(result, t => t.Title == "Test Item 2");
      Assert.DoesNotContain(result, t => t.Title == "Test Item 3");
    }

    [Fact]
    public async Task AddAsync_AddsTodoItemToDatabase()
    {
      // Arrange
      var todoItem = new TodoItem { Title = "Test Item", UserId = "user1" };

      // Act
      var result = await _repository.AddAsync(todoItem);

      // Assert
      Assert.NotEqual(0, result.Id);
      Assert.Equal(1, _context.TodoItems.Count());
      var dbTodoItem = await _context.TodoItems.FindAsync(result.Id);
      Assert.NotNull(dbTodoItem);
      Assert.Equal(todoItem.Title, dbTodoItem.Title);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesTodoItemInDatabase()
    {
      // Arrange
      var todoItem = new TodoItem { Title = "Test Item", UserId = "user1" };
      _context.TodoItems.Add(todoItem);
      await _context.SaveChangesAsync();

      // Act
      todoItem.Title = "Updated Test Item";
      await _repository.UpdateAsync(todoItem);

      // Assert
      var dbTodoItem = await _context.TodoItems.FindAsync(todoItem.Id);
      Assert.NotNull(dbTodoItem);
      Assert.Equal("Updated Test Item", dbTodoItem.Title);
    }

    [Fact]
    public async Task DeleteAsync_RemovesTodoItemFromDatabase()
    {
      // Arrange
      var todoItem = new TodoItem { Title = "Test Item", UserId = "user1" };
      _context.TodoItems.Add(todoItem);
      await _context.SaveChangesAsync();

      // Act
      await _repository.DeleteAsync(todoItem.Id);

      // Assert
      Assert.Equal(0, _context.TodoItems.Count());
      var dbTodoItem = await _context.TodoItems.FindAsync(todoItem.Id);
      Assert.Null(dbTodoItem);
    }


    [Fact]
    public async Task GetPagedAsync_ReturnsCorrectItemsAndNextCursor()
    {
      // Arrange
      var options = new DbContextOptionsBuilder<TodoDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;

      using (var context = new TodoDbContext(options))
      {
        var userId = "testUser";
        var items = new List<TodoItem>
                {
                    new TodoItem { Id = 1, Title = "Item 1", UserId = userId },
                    new TodoItem { Id = 2, Title = "Item 2", UserId = userId },
                    new TodoItem { Id = 3, Title = "Item 3", UserId = userId },
                    new TodoItem { Id = 4, Title = "Item 4", UserId = userId },
                    new TodoItem { Id = 5, Title = "Item 5", UserId = userId },
                };
        context.TodoItems.AddRange(items);
        context.SaveChanges();
      }

      using (var context = new TodoDbContext(options))
      {
        var mockLogger = new Mock<ILogger<TodoItemRepository>>();
        var repository = new TodoItemRepository(context, mockLogger.Object);

        // Act
        var result = await repository.GetPagedAsync(
            "testUser",
            new PaginationRequest(2, null) // Request 2 items per page, starting from the beginning
        );

        // Assert
        Assert.Equal(2, result.Items.Count());
        Assert.Equal("Item 1", result.Items.First().Title);
        Assert.Equal("Item 2", result.Items.Last().Title);
        Assert.NotNull(result.NextCursor);
        Assert.True(result.HasNextPage);

        // Act again with the next cursor
        result = await repository.GetPagedAsync(
            "testUser",
            new PaginationRequest(2, result.NextCursor)
        );

        // Assert
        Assert.Equal(2, result.Items.Count());
        Assert.Equal("Item 3", result.Items.First().Title);
        Assert.Equal("Item 4", result.Items.Last().Title);
        Assert.NotNull(result.NextCursor);
        Assert.True(result.HasNextPage);

        // Act one last time to get the last page
        result = await repository.GetPagedAsync(
            "testUser",
            new PaginationRequest(2, result.NextCursor)
        );

        // Assert
        Assert.Single(result.Items);
        Assert.Equal("Item 5", result.Items.First().Title);
        Assert.Null(result.NextCursor);
        Assert.False(result.HasNextPage);
      }
    }

    [Fact]
    public async Task GetPagedAsync_WithNonExistentUser_ReturnsEmptyResult()
    {
      // Arrange
      var options = new DbContextOptionsBuilder<TodoDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;

      using (var context = new TodoDbContext(options))
      {
        var mockLogger = new Mock<ILogger<TodoItemRepository>>();
        var repository = new TodoItemRepository(context, mockLogger.Object);

        // Act
        var result = await repository.GetPagedAsync(
            "nonExistentUser",
            new PaginationRequest(10, null)
        );

        // Assert
        Assert.Empty(result.Items);
        Assert.Null(result.NextCursor);
        Assert.False(result.HasNextPage);
      }
    }

    public void Dispose()
    {
      _context.Database.EnsureDeleted();
      _context.Dispose();
    }
  }
}