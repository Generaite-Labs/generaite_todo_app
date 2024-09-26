using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using ToDo.Domain.Entities;
using ToDo.Infrastructure;
using ToDo.Infrastructure.Repositories;
using Xunit;

namespace ToDo.Infrastructure.Tests
{
  public class TodoItemRepositoryTests : IDisposable
  {
    private readonly TodoDbContext _context;
    private readonly TodoItemRepository _repository;

    public TodoItemRepositoryTests()
    {
      var options = new DbContextOptionsBuilder<TodoDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;

      _context = new TodoDbContext(options);
      _repository = new TodoItemRepository(_context);
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
    public async Task GetByListIdAsync_ReturnsCorrectTodoItems()
    {
      // Arrange
      var todoItemList = new TodoItemList { Name = "Test List", UserId = "user1" };
      _context.TodoItemLists.Add(todoItemList);
      await _context.SaveChangesAsync();

      var todoItem1 = new TodoItem { Title = "Test Item 1", UserId = "user1", TodoItemListId = todoItemList.Id };
      var todoItem2 = new TodoItem { Title = "Test Item 2", UserId = "user1", TodoItemListId = todoItemList.Id };
      var todoItem3 = new TodoItem { Title = "Test Item 3", UserId = "user1", TodoItemListId = null };
      _context.TodoItems.AddRange(todoItem1, todoItem2, todoItem3);
      await _context.SaveChangesAsync();

      // Act
      var result = await _repository.GetByListIdAsync(todoItemList.Id);

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

    public void Dispose()
    {
      _context.Database.EnsureDeleted();
      _context.Dispose();
    }
  }
}