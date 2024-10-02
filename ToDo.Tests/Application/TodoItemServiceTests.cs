using FluentAssertions;
using Moq;
using ToDo.Domain.Entities;
using ToDo.Domain.Interfaces;
using ToDo.Domain.Common;
using ToDo.Application.Services;
using ToDo.Application.DTOs;

namespace ToDo.Tests.Application
{
  public class TodoItemServiceTests
  {
    private readonly Mock<ITodoItemRepository> _mockRepo;
    private readonly ITodoItemService _service;

    public TodoItemServiceTests()
    {
      _mockRepo = new Mock<ITodoItemRepository>();
      _service = new TodoItemService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTodoItemDto_WhenItemExists()
    {
      // Arrange
      var todoItem = new TodoItem { Id = 1, Title = "Test Item", UserId = "user1" };
      _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(todoItem);

      // Act
      var result = await _service.GetByIdAsync(1);

      // Assert
      result.Should().NotBeNull();
      result!.Id.Should().Be(1);
      result.Title.Should().Be("Test Item");
      result.UserId.Should().Be("user1");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllItemsAsDto()
    {
      // Arrange
      var todoItems = new List<TodoItem>
            {
                new TodoItem { Id = 1, Title = "Item 1", UserId = "user1" },
                new TodoItem { Id = 2, Title = "Item 2", UserId = "user2" }
            };
      _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(todoItems);

      // Act
      var result = await _service.GetAllAsync();

      // Assert
      result.Should().HaveCount(2);
      result.Should().Contain(dto => dto.Id == 1 && dto.Title == "Item 1" && dto.UserId == "user1");
      result.Should().Contain(dto => dto.Id == 2 && dto.Title == "Item 2" && dto.UserId == "user2");
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnCreatedTodoItemDto()
    {
      // Arrange
      var createDto = new CreateTodoItemDto
      {
        Title = "New Item",
        UserId = "user1",
        Status = TodoItemStatus.TODO
      };
      var createdItem = new TodoItem
      {
        Id = 1,
        Title = "New Item",
        UserId = "user1",
        Status = TodoItemStatus.TODO,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
      };
      _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<TodoItem>())).ReturnsAsync(createdItem);

      // Act
      var result = await _service.CreateAsync(createDto);

      // Assert
      result.Should().NotBeNull();
      result.Id.Should().Be(1);
      result.Title.Should().Be("New Item");
      result.UserId.Should().Be("user1");
      result.Status.Should().Be(TodoItemStatus.TODO);
      result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
      result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnUpdatedTodoItemDto()
    {
      // Arrange
      var existingItem = new TodoItem
      {
        Id = 1,
        Title = "Old Title",
        UserId = "user1",
        Status = TodoItemStatus.TODO
      };
      var updateDto = new UpdateTodoItemDto
      {
        Title = "Updated Title",
        Status = TodoItemStatus.IN_PROGRESS
      };
      _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingItem);

      // Act
      var result = await _service.UpdateAsync(1, updateDto);

      // Assert
      result.Should().NotBeNull();
      result.Id.Should().Be(1);
      result.Title.Should().Be("Updated Title");
      result.Status.Should().Be(TodoItemStatus.IN_PROGRESS);
      result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallRepositoryDeleteMethod()
    {
      // Arrange
      var idToDelete = 1;

      // Act
      await _service.DeleteAsync(idToDelete);

      // Assert
      _mockRepo.Verify(repo => repo.DeleteAsync(idToDelete), Times.Once);
    }


    [Fact]
    public async Task GetPagedAsync_ReturnsCorrectPaginatedResultDto()
    {
      // Arrange
      var userId = "testUser";
      var paginationRequestDto = new PaginationRequestDto { Limit = 2, Cursor = null };
      var domainPaginationRequest = new PaginationRequest(paginationRequestDto.Limit, paginationRequestDto.Cursor);

      var todoItems = new List<TodoItem>
            {
                new TodoItem { Id = 1, Title = "Task 1", UserId = userId },
                new TodoItem { Id = 2, Title = "Task 2", UserId = userId }
            };

      var domainPaginatedResult = new PaginatedResult<TodoItem>(todoItems, "nextPageCursor");

      _mockRepo.Setup(repo => repo.GetPagedAsync(userId, It.IsAny<PaginationRequest>()))
                     .ReturnsAsync(domainPaginatedResult);

      // Act
      var result = await _service.GetPagedAsync(userId, paginationRequestDto);

      // Assert
      Assert.NotNull(result);
      Assert.Equal(2, result.Items.Count);
      Assert.Equal("Task 1", result.Items[0].Title);
      Assert.Equal("Task 2", result.Items[1].Title);
      Assert.Equal("nextPageCursor", result.NextCursor);
      Assert.True(result.HasNextPage);

      _mockRepo.Verify(repo => repo.GetPagedAsync(userId, It.Is<PaginationRequest>(pr =>
          pr.Limit == paginationRequestDto.Limit && pr.Cursor == paginationRequestDto.Cursor)), Times.Once);
    }

    [Fact]
    public async Task GetPagedAsync_WithNullNextCursor_ReturnsResultWithNoNextPage()
    {
      // Arrange
      var userId = "testUser";
      var paginationRequestDto = new PaginationRequestDto { Limit = 2, Cursor = "someCursor" };
      var domainPaginationRequest = new PaginationRequest(paginationRequestDto.Limit, paginationRequestDto.Cursor);

      var todoItems = new List<TodoItem>
            {
                new TodoItem { Id = 3, Title = "Task 3", UserId = userId }
            };

      var domainPaginatedResult = new PaginatedResult<TodoItem>(todoItems, null);

      _mockRepo.Setup(repo => repo.GetPagedAsync(userId, It.IsAny<PaginationRequest>()))
                     .ReturnsAsync(domainPaginatedResult);

      // Act
      var result = await _service.GetPagedAsync(userId, paginationRequestDto);

      // Assert
      Assert.NotNull(result);
      Assert.Single(result.Items);
      Assert.Equal("Task 3", result.Items[0].Title);
      Assert.Null(result.NextCursor);
      Assert.False(result.HasNextPage);

      _mockRepo.Verify(repo => repo.GetPagedAsync(userId, It.Is<PaginationRequest>(pr =>
          pr.Limit == paginationRequestDto.Limit && pr.Cursor == paginationRequestDto.Cursor)), Times.Once);
    }

    [Fact]
    public async Task GetPagedAsync_WithEmptyResult_ReturnsEmptyPaginatedResultDto()
    {
      // Arrange
      var userId = "testUser";
      var paginationRequestDto = new PaginationRequestDto { Limit = 2, Cursor = null };
      var domainPaginationRequest = new PaginationRequest(paginationRequestDto.Limit, paginationRequestDto.Cursor);

      var emptyList = new List<TodoItem>();
      var domainPaginatedResult = new PaginatedResult<TodoItem>(emptyList, null);

      _mockRepo.Setup(repo => repo.GetPagedAsync(userId, It.IsAny<PaginationRequest>()))
                     .ReturnsAsync(domainPaginatedResult);

      // Act
      var result = await _service.GetPagedAsync(userId, paginationRequestDto);

      // Assert
      Assert.NotNull(result);
      Assert.Empty(result.Items);
      Assert.Null(result.NextCursor);
      Assert.False(result.HasNextPage);

      _mockRepo.Verify(repo => repo.GetPagedAsync(userId, It.Is<PaginationRequest>(pr =>
          pr.Limit == paginationRequestDto.Limit && pr.Cursor == paginationRequestDto.Cursor)), Times.Once);
    }
  }
}