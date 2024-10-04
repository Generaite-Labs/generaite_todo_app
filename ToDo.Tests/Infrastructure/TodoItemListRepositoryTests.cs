using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Entities;
using ToDo.Domain.Common;
using ToDo.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace ToDo.Infrastructure.Tests
{
  public class TodoItemListRepositoryTests : IDisposable
  {
    private readonly TodoDbContext _context;
    private readonly TodoItemListRepository _repository;
    private readonly Mock<ILogger<TodoItemListRepository>> _mockLogger;

    public TodoItemListRepositoryTests()
    {
      var options = new DbContextOptionsBuilder<TodoDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;

      _context = new TodoDbContext(options);
      _mockLogger = new Mock<ILogger<TodoItemListRepository>>();
      _repository = new TodoItemListRepository(_context, _mockLogger.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectTodoItemList()
    {
      // Arrange
      var todoItemList = new TodoItemList { Name = "Test List", UserId = "1" };
      _context.TodoItemLists.Add(todoItemList);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.GetByIdAsync(todoItemList.Id);

      // Assert
      Assert.NotNull(result);
      Assert.Equal(todoItemList.Id, result.Id);
      Assert.Equal(todoItemList.Name, result.Name);
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
    public async Task GetAllAsync_ReturnsAllTodoItemLists()
    {
      // Arrange
      var todoItemList1 = new TodoItemList { Name = "Test List 1", UserId = "1" };
      var todoItemList2 = new TodoItemList { Name = "Test List 2", UserId = "1" };
      _context.TodoItemLists.AddRange(todoItemList1, todoItemList2);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.GetAllAsync();

      // Assert
      Assert.Equal(2, result.Count());
      Assert.Contains(result, t => t.Name == "Test List 1");
      Assert.Contains(result, t => t.Name == "Test List 2");
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsCorrectTodoItemLists()
    {
      // Arrange
      var todoItemList1 = new TodoItemList { Name = "Test List 1", UserId = "user1" };
      var todoItemList2 = new TodoItemList { Name = "Test List 2", UserId = "user1" };
      var todoItemList3 = new TodoItemList { Name = "Test List 3", UserId = "user2" };
      _context.TodoItemLists.AddRange(todoItemList1, todoItemList2, todoItemList3);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.GetByUserIdAsync("user1");

      // Assert
      Assert.Equal(2, result.Count());
      Assert.Contains(result, t => t.Name == "Test List 1");
      Assert.Contains(result, t => t.Name == "Test List 2");
      Assert.DoesNotContain(result, t => t.Name == "Test List 3");
    }

    [Fact]
    public async Task AddAsync_AddsTodoItemListToDatabase()
    {
      // Arrange
      var todoItemList = new TodoItemList { Name = "Test List", UserId = "1" };

      // Act
      var result = await _repository.AddAsync(todoItemList);

      // Assert
      Assert.NotEqual(0, result.Id);
      Assert.Equal(1, _context.TodoItemLists.Count());
      var dbTodoItemList = await _context.TodoItemLists.FindAsync(result.Id);
      Assert.NotNull(dbTodoItemList);
      Assert.Equal(todoItemList.Name, dbTodoItemList.Name);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesTodoItemListInDatabase()
    {
      // Arrange
      var todoItemList = new TodoItemList { Name = "Test List", UserId = "1" };
      _context.TodoItemLists.Add(todoItemList);
      await _context.SaveChangesAsync();

      // Act
      todoItemList.Name = "Updated Test List";
      await _repository.UpdateAsync(todoItemList);

      // Assert
      var dbTodoItemList = await _context.TodoItemLists.FindAsync(todoItemList.Id);
      Assert.NotNull(dbTodoItemList);
      Assert.Equal("Updated Test List", dbTodoItemList.Name);
    }

    [Fact]
    public async Task DeleteAsync_RemovesTodoItemListFromDatabase()
    {
      // Arrange
      var todoItemList = new TodoItemList { Name = "Test List", UserId = "1" };
      _context.TodoItemLists.Add(todoItemList);
      await _context.SaveChangesAsync();

      // Act
      await _repository.DeleteAsync(todoItemList.Id);

      // Assert
      Assert.Equal(0, _context.TodoItemLists.Count());
      var dbTodoItemList = await _context.TodoItemLists.FindAsync(todoItemList.Id);
      Assert.Null(dbTodoItemList);
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
        var lists = new List<TodoItemList>
                {
                    new TodoItemList { Id = 1, Name = "List 1", UserId = userId },
                    new TodoItemList { Id = 2, Name = "List 2", UserId = userId },
                    new TodoItemList { Id = 3, Name = "List 3", UserId = userId },
                    new TodoItemList { Id = 4, Name = "List 4", UserId = userId },
                    new TodoItemList { Id = 5, Name = "List 5", UserId = userId },
                };
        context.TodoItemLists.AddRange(lists);
        context.SaveChanges();
      }

      using (var context = new TodoDbContext(options))
      {
        var mockLogger = new Mock<ILogger<TodoItemListRepository>>();
        var repository = new TodoItemListRepository(context, mockLogger.Object);

        // Act
        var result = await repository.GetPagedAsync(
            "testUser",
            new PaginationRequest(2, null) // Request 2 items per page, starting from the beginning
        );

        // Assert
        Assert.Equal(2, result.Items.Count());
        Assert.Equal("List 1", result.Items.First().Name);
        Assert.Equal("List 2", result.Items.Last().Name);
        Assert.NotNull(result.NextCursor);
        Assert.True(result.HasNextPage);

        // Act again with the next cursor
        result = await repository.GetPagedAsync(
            "testUser",
            new PaginationRequest(2, result.NextCursor)
        );

        // Assert
        Assert.Equal(2, result.Items.Count());
        Assert.Equal("List 3", result.Items.First().Name);
        Assert.Equal("List 4", result.Items.Last().Name);
        Assert.NotNull(result.NextCursor);
        Assert.True(result.HasNextPage);

        // Act one last time to get the last page
        result = await repository.GetPagedAsync(
            "testUser",
            new PaginationRequest(2, result.NextCursor)
        );

        // Assert
        Assert.Single(result.Items);
        Assert.Equal("List 5", result.Items.First().Name);
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
        var mockLogger = new Mock<ILogger<TodoItemListRepository>>();
        var repository = new TodoItemListRepository(context, mockLogger.Object);

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

    [Fact]
    public async Task GetPagedAsync_WithDifferentPageSizes_ReturnsCorrectResults()
    {
      // Arrange
      var options = new DbContextOptionsBuilder<TodoDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;

      using (var context = new TodoDbContext(options))
      {
        var userId = "testUser";
        var lists = Enumerable.Range(1, 10).Select(i => new TodoItemList { Id = i, Name = $"List {i}", UserId = userId }).ToList();
        context.TodoItemLists.AddRange(lists);
        context.SaveChanges();
      }

      using (var context = new TodoDbContext(options))
      {
        var mockLogger = new Mock<ILogger<TodoItemListRepository>>();
        var repository = new TodoItemListRepository(context, mockLogger.Object);

        // Act & Assert for page size 3
        var result = await repository.GetPagedAsync("testUser", new PaginationRequest(3, null));
        Assert.Equal(3, result.Items.Count());
        Assert.NotNull(result.NextCursor);

        // Act & Assert for page size 5
        result = await repository.GetPagedAsync("testUser", new PaginationRequest(5, null));
        Assert.Equal(5, result.Items.Count());
        Assert.NotNull(result.NextCursor);

        // Act & Assert for page size larger than remaining items
        result = await repository.GetPagedAsync("testUser", new PaginationRequest(10, result.NextCursor));
        Assert.Equal(5, result.Items.Count());
        Assert.Null(result.NextCursor);
      }
    }

    private void VerifyLog(LogLevel level, string messageContains)
    {
      _mockLogger.Verify(
          x => x.Log(
              level,
              It.IsAny<EventId>(),
              It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains(messageContains)),
              It.IsAny<Exception>(),
              (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
          Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_LogsInformation()
    {
      // Arrange
      var todoItemList = new TodoItemList { Name = "Test List", UserId = "1" };
      _context.TodoItemLists.Add(todoItemList);
      await _context.SaveChangesAsync();

      // Act
      await _repository.GetByIdAsync(todoItemList.Id);

      // Assert
      VerifyLog(LogLevel.Information, $"Getting TodoItemList by ID: {todoItemList.Id}");
    }

    [Fact]
    public async Task GetAllAsync_LogsInformationAndPerformance()
    {
      // Arrange
      var todoItemList1 = new TodoItemList { Name = "Test List 1", UserId = "1" };
      var todoItemList2 = new TodoItemList { Name = "Test List 2", UserId = "1" };
      _context.TodoItemLists.AddRange(todoItemList1, todoItemList2);
      await _context.SaveChangesAsync();

      // Act
      await _repository.GetAllAsync();

      // Assert
      VerifyLog(LogLevel.Information, "Getting all TodoItemLists");
      VerifyLog(LogLevel.Information, "Retrieved 2 TodoItemLists. Took");
    }

    [Fact]
    public async Task AddAsync_LogsInformation()
    {
      // Arrange
      var todoItemList = new TodoItemList { Name = "Test List", UserId = "1" };

      // Act
      await _repository.AddAsync(todoItemList);

      // Assert
      VerifyLog(LogLevel.Information, "Adding new TodoItemList");
      VerifyLog(LogLevel.Information, $"Added new TodoItemList with ID: {todoItemList.Id}");
    }

    [Fact]
    public async Task GetByUserIdAsync_LogsInformationAndPerformance()
    {
      // Arrange
      var userId = "user1";
      var todoItemList1 = new TodoItemList { Name = "Test List 1", UserId = userId };
      var todoItemList2 = new TodoItemList { Name = "Test List 2", UserId = userId };
      _context.TodoItemLists.AddRange(todoItemList1, todoItemList2);
      await _context.SaveChangesAsync();

      // Act
      await _repository.GetByUserIdAsync(userId);

      // Assert
      VerifyLog(LogLevel.Information, $"Getting TodoItemLists for user: {userId}");
      VerifyLog(LogLevel.Information, $"Retrieved 2 TodoItemLists for user {userId}. Took");
    }

    [Fact]
    public async Task UpdateAsync_LogsInformation()
    {
      // Arrange
      var todoItemList = new TodoItemList { Name = "Test List", UserId = "1" };
      _context.TodoItemLists.Add(todoItemList);
      await _context.SaveChangesAsync();

      // Act
      todoItemList.Name = "Updated Test List";
      await _repository.UpdateAsync(todoItemList);

      // Assert
      VerifyLog(LogLevel.Information, $"Updating TodoItemList: ");
      VerifyLog(LogLevel.Information, $"Updated TodoItemList with ID: {todoItemList.Id}");
    }

    [Fact]
    public async Task DeleteAsync_LogsInformation()
    {
      // Arrange
      var todoItemList = new TodoItemList { Name = "Test List", UserId = "1" };
      _context.TodoItemLists.Add(todoItemList);
      await _context.SaveChangesAsync();

      // Act
      await _repository.DeleteAsync(todoItemList.Id);

      // Assert
      VerifyLog(LogLevel.Information, $"Deleting TodoItemList with ID: {todoItemList.Id}");
      VerifyLog(LogLevel.Information, $"Deleted TodoItemList with ID: {todoItemList.Id}");
    }

    [Fact]
    public async Task GetPagedAsync_LogsInformationAndPerformance()
    {
      // Arrange
      var userId = "testUser";
      var lists = new List<TodoItemList>
      {
          new TodoItemList { Id = 1, Name = "List 1", UserId = userId },
          new TodoItemList { Id = 2, Name = "List 2", UserId = userId },
          new TodoItemList { Id = 3, Name = "List 3", UserId = userId },
      };
      _context.TodoItemLists.AddRange(lists);
      await _context.SaveChangesAsync();

      // Act
      await _repository.GetPagedAsync(userId, new PaginationRequest(2, null));

      // Assert
      VerifyLog(LogLevel.Information, $"Getting paged TodoItemLists for user: {userId}");
      VerifyLog(LogLevel.Information, $"Retrieved paged TodoItemLists for user: {userId}. Took");
    }

    public void Dispose()
    {
      _context.Database.EnsureDeleted();
      _context.Dispose();
    }
  }
}