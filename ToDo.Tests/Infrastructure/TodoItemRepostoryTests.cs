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
    private readonly ApplicationDbContext _context;
    private readonly TodoItemRepository _repository;
    private readonly Mock<ILogger<TodoItemRepository>> _mockLogger;

    public TodoItemRepositoryTests()
    {
      var options = new DbContextOptionsBuilder<ApplicationDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;

      _context = new ApplicationDbContext(options);
      _mockLogger = new Mock<ILogger<TodoItemRepository>>();
      _repository = new TodoItemRepository(_context, _mockLogger.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectTodoItem()
    {
      // Arrange
      var todoItem = TodoItem.CreateTodoItem("Test Item", null, "user1", null);
      var id = Guid.NewGuid();
      var idProperty = todoItem.GetType().GetProperty("Id");
      if (idProperty != null)
      {
        idProperty.SetValue(todoItem, id);
      }
      _context.TodoItems.Add(todoItem);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.GetByIdAsync(id);

      // Assert
      Assert.NotNull(result);
      Assert.Equal(id, result.Id);
      Assert.Equal(todoItem.Title, result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNullForNonexistentId()
    {
      // Act
      var result = await _repository.GetByIdAsync(Guid.NewGuid());

      // Assert
      Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllTodoItems()
    {
      // Arrange
      var todoItem1 = TodoItem.CreateTodoItem("Test Item 1", null, "user1", null);
      var todoItem2 = TodoItem.CreateTodoItem("Test Item 2", null, "user1", null);
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
    public async Task GetAllAsync_ReturnsEmptyList_WhenNoTodoItemsExist()
    {
      // Arrange
      // The database is empty by default, so we don't need to add any items

      // Act
      var result = await _repository.GetAllAsync();

      // Assert
      Assert.Empty(result);
      Assert.IsType<List<TodoItem>>(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsCorrectTodoItems()
    {
      // Arrange
      var todoItem1 = TodoItem.CreateTodoItem("Test Item 1", null, "user1", null);
      var todoItem2 = TodoItem.CreateTodoItem("Test Item 2", null, "user1", null);
      var todoItem3 = TodoItem.CreateTodoItem("Test Item 3", null, "user2", null);
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
      var todoItem = TodoItem.CreateTodoItem("Test Item", null, "user1", null);

      // Act
      var result = await _repository.AddAsync(todoItem);

      // Assert
      Assert.NotEqual(Guid.Empty, result.Id);
      Assert.Equal(1, _context.TodoItems.Count());
      var dbTodoItem = await _context.TodoItems.FindAsync(result.Id);
      Assert.NotNull(dbTodoItem);
      Assert.Equal(todoItem.Title, dbTodoItem.Title);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesTodoItemInDatabase()
    {
      // Arrange
      var todoItem = TodoItem.CreateTodoItem("Test Item", null, "user1", null);
      _context.TodoItems.Add(todoItem);
      await _context.SaveChangesAsync();

      // Act
      todoItem.UpdateTodoItem("Updated Test Item", null, null);
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
      var todoItem = TodoItem.CreateTodoItem("Test Item", null, "user1", null);
      var id = Guid.NewGuid();
      var idProperty = todoItem.GetType().GetProperty("Id");
      if (idProperty != null)
      {
        idProperty.SetValue(todoItem, id);
      }
      _context.TodoItems.Add(todoItem);
      await _context.SaveChangesAsync();

      // Act
      await _repository.DeleteAsync(id);

      // Assert
      Assert.Equal(0, _context.TodoItems.Count());
      var dbTodoItem = await _context.TodoItems.FindAsync(id);
      Assert.Null(dbTodoItem);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsCorrectItemsAndNextCursor()
    {
      // Arrange
      var userId = "testUser";
      var items = new List<TodoItem>
      {
        TodoItem.CreateTodoItem("Item 1", null, userId, null),
        TodoItem.CreateTodoItem("Item 2", null, userId, null),
        TodoItem.CreateTodoItem("Item 3", null, userId, null),
        TodoItem.CreateTodoItem("Item 4", null, userId, null),
        TodoItem.CreateTodoItem("Item 5", null, userId, null)
      };

      // Set specific GUIDs to ensure ordering
      for (int i = 0; i < items.Count; i++)
      {
        var idProperty = items[i].GetType().GetProperty("Id");
        if (idProperty != null)
        {
          // Create sequential GUIDs to maintain order
          var guid = Guid.Parse($"00000000-0000-0000-0000-{(i + 1):D12}");
          idProperty.SetValue(items[i], guid);
        }
      }

      _context.TodoItems.AddRange(items);
      await _context.SaveChangesAsync();

      // Act - First Page
      var firstPageResult = await _repository.GetPagedAsync(
          userId,
          new PaginationRequest(2, null) // Request 2 items per page, starting from the beginning
      );

      // Assert - First Page
      Assert.Equal(2, firstPageResult.Items.Count());
      Assert.Equal("Item 1", firstPageResult.Items.First().Title);
      Assert.Equal("Item 2", firstPageResult.Items.Last().Title);
      Assert.NotNull(firstPageResult.NextCursor);
      Assert.True(firstPageResult.HasNextPage);

      // Act - Second Page
      var secondPageResult = await _repository.GetPagedAsync(
          userId,
          new PaginationRequest(2, firstPageResult.NextCursor)
      );

      // Assert - Second Page
      Assert.Equal(2, secondPageResult.Items.Count());
      Assert.Equal("Item 3", secondPageResult.Items.First().Title);
      Assert.Equal("Item 4", secondPageResult.Items.Last().Title);
      Assert.NotNull(secondPageResult.NextCursor);
      Assert.True(secondPageResult.HasNextPage);

      // Act - Last Page
      var lastPageResult = await _repository.GetPagedAsync(
          userId,
          new PaginationRequest(2, secondPageResult.NextCursor)
      );

      // Assert - Last Page
      Assert.Single(lastPageResult.Items);
      Assert.Equal("Item 5", lastPageResult.Items.First().Title);
      Assert.Null(lastPageResult.NextCursor);
      Assert.False(lastPageResult.HasNextPage);
    }

    [Fact]
    public async Task GetPagedAsync_WithNonExistentUser_ReturnsEmptyResult()
    {
      // Arrange
      var options = new DbContextOptionsBuilder<ApplicationDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;

      using (var context = new ApplicationDbContext(options))
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
