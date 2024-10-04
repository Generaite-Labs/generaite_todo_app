using Moq;
using FluentAssertions;
using ToDo.Domain.Entities;
using ToDo.Domain.Interfaces;
using ToDo.Application.Services;
using ToDo.Application.DTOs;
using ToDo.Application.Exceptions;
using Microsoft.Extensions.Logging;
using ToDo.Domain.Common;
using AutoMapper;
using System.Linq;

namespace ToDo.Tests.Application
{
  public class TodoItemServiceTests
  {
    private readonly Mock<ITodoItemRepository> _mockRepo;
    private readonly Mock<ILogger<TodoItemService>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ITodoItemService _service;

    public TodoItemServiceTests()
    {
      _mockRepo = new Mock<ITodoItemRepository>();
      _mockLogger = new Mock<ILogger<TodoItemService>>();
      _mockMapper = new Mock<IMapper>();
      _service = new TodoItemService(_mockRepo.Object, _mockLogger.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTodoItemDto_WhenItemExists()
    {
      // Arrange
      var todoItem = new TodoItem { Id = 1, Title = "Test Item", UserId = "user1" };
      var todoItemDto = new TodoItemDto { Id = 1, Title = "Test Item", UserId = "user1" };
      _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(todoItem);
      _mockMapper.Setup(mapper => mapper.Map<TodoItemDto>(todoItem)).Returns(todoItemDto);

      // Act
      var result = await _service.GetByIdAsync(1);

      // Assert
      result.Should().NotBeNull();
      result!.Id.Should().Be(1);
      result.Title.Should().Be("Test Item");
      result.UserId.Should().Be("user1");

      // Verify logging
      _mockLogger.Verify(
          x => x.Log(
              LogLevel.Information,
              It.IsAny<EventId>(),
              It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Getting TodoItem by ID: 1")),
              It.IsAny<Exception>(),
              It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
          Times.Once);
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
      var todoItemDtos = new List<TodoItemDto>
            {
                new TodoItemDto { Id = 1, Title = "Item 1", UserId = "user1" },
                new TodoItemDto { Id = 2, Title = "Item 2", UserId = "user2" }
            };
      _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(todoItems);
      _mockMapper.Setup(mapper => mapper.Map<IEnumerable<TodoItemDto>>(todoItems)).Returns(todoItemDtos);

      // Act
      var result = await _service.GetAllAsync();

      // Assert
      result.Should().HaveCount(2);
      result.Should().Contain(dto => dto.Id == 1 && dto.Title == "Item 1" && dto.UserId == "user1");
      result.Should().Contain(dto => dto.Id == 2 && dto.Title == "Item 2" && dto.UserId == "user2");

      // Verify logging
      _mockLogger.Verify(
          x => x.Log(
              LogLevel.Information,
              It.IsAny<EventId>(),
              It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Getting all TodoItems")),
              It.IsAny<Exception>(),
              It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
          Times.Once);
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
      var createdItemDto = new TodoItemDto
      {
        Id = 1,
        Title = "New Item",
        UserId = "user1",
        Status = TodoItemStatus.TODO,
        CreatedAt = createdItem.CreatedAt,
        UpdatedAt = createdItem.UpdatedAt
      };
      _mockMapper.Setup(mapper => mapper.Map<TodoItem>(createDto)).Returns(createdItem);
      _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<TodoItem>())).ReturnsAsync(createdItem);
      _mockMapper.Setup(mapper => mapper.Map<TodoItemDto>(createdItem)).Returns(createdItemDto);

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

      // Verify logging
      _mockLogger.Verify(
          x => x.Log(
              LogLevel.Information,
              It.IsAny<EventId>(),
              It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Creating new TodoItem")),
              It.IsAny<Exception>(),
              It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
          Times.Once);
      _mockLogger.Verify(
          x => x.Log(
              LogLevel.Information,
              It.IsAny<EventId>(),
              It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Created TodoItem with ID: 1")),
              It.IsAny<Exception>(),
              It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
          Times.Once);
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
      var updatedItem = new TodoItem
      {
        Id = 1,
        Title = "Updated Title",
        UserId = "user1",
        Status = TodoItemStatus.IN_PROGRESS,
        UpdatedAt = DateTime.UtcNow
      };
      var updatedItemDto = new TodoItemDto
      {
        Id = 1,
        Title = "Updated Title",
        UserId = "user1",
        Status = TodoItemStatus.IN_PROGRESS,
        UpdatedAt = updatedItem.UpdatedAt
      };
      _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingItem);
      _mockMapper.Setup(mapper => mapper.Map(updateDto, existingItem)).Callback<UpdateTodoItemDto, TodoItem>((src, dest) =>
      {
        dest.Title = src.Title;
        dest.Status = src.Status;
      });
      _mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<TodoItem>())).Returns(Task.CompletedTask);
      _mockMapper.Setup(mapper => mapper.Map<TodoItemDto>(It.IsAny<TodoItem>())).Returns(updatedItemDto);

      // Act
      var result = await _service.UpdateAsync(1, updateDto);

      // Assert
      result.Should().NotBeNull();
      result.Id.Should().Be(1);
      result.Title.Should().Be("Updated Title");
      result.Status.Should().Be(TodoItemStatus.IN_PROGRESS);
      result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

      // Verify that UpdateAsync was called with the correct item
      _mockRepo.Verify(repo => repo.UpdateAsync(It.Is<TodoItem>(item =>
        item.Id == 1 &&
        item.Title == "Updated Title" &&
        item.Status == TodoItemStatus.IN_PROGRESS)),
      Times.Once);
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

      var todoItemDtos = new List<TodoItemDto>
            {
                new TodoItemDto { Id = 1, Title = "Task 1", UserId = userId },
                new TodoItemDto { Id = 2, Title = "Task 2", UserId = userId }
            };

      var domainPaginatedResult = new PaginatedResult<TodoItem>(todoItems, "nextPageCursor");

      _mockRepo.Setup(repo => repo.GetPagedAsync(userId, It.IsAny<PaginationRequest>()))
                     .ReturnsAsync(domainPaginatedResult);
      _mockMapper.Setup(mapper => mapper.Map<List<TodoItemDto>>(todoItems)).Returns(todoItemDtos);

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

      var todoItemDtos = new List<TodoItemDto>
            {
                new TodoItemDto { Id = 3, Title = "Task 3", UserId = userId }
            };

      var domainPaginatedResult = new PaginatedResult<TodoItem>(todoItems, null);

      _mockRepo.Setup(repo => repo.GetPagedAsync(userId, It.IsAny<PaginationRequest>()))
                     .ReturnsAsync(domainPaginatedResult);
      _mockMapper.Setup(mapper => mapper.Map<List<TodoItemDto>>(It.IsAny<List<TodoItem>>())).Returns(todoItemDtos);

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
    public async Task GetPagedAsync_WithEmptyResult_ThrowsInvalidTodoItemMappingException()
    {
      // Arrange
      var userId = "testUser";
      var paginationRequestDto = new PaginationRequestDto { Limit = 2, Cursor = null };
      var emptyList = new List<TodoItem>();
      var domainPaginatedResult = new PaginatedResult<TodoItem>(emptyList, null);

      _mockRepo.Setup(repo => repo.GetPagedAsync(userId, It.IsAny<PaginationRequest>()))
                       .ReturnsAsync(domainPaginatedResult);
      _mockMapper.Setup(mapper => mapper.Map<List<TodoItemDto>>(It.IsAny<List<TodoItem>>())).Returns(new List<TodoItemDto>());

      // Act & Assert
      await Assert.ThrowsAsync<InvalidTodoItemMappingException>(() => _service.GetPagedAsync(userId, paginationRequestDto));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowTodoItemNotFoundException_WhenItemDoesNotExist()
    {
      // Arrange
      _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((TodoItem?)null);

      // Act & Assert
      await Assert.ThrowsAsync<TodoItemNotFoundException>(() => _service.GetByIdAsync(1));
    }

    [Fact]
    public async Task GetAllAsync_ShouldThrowInvalidTodoItemMappingException_WhenMappingFails()
    {
      // Arrange
      var todoItems = new List<TodoItem> { new TodoItem { Id = 1, Title = "Test Item", UserId = "user123" } };
      _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(todoItems);
      _mockMapper.Setup(mapper => mapper.Map<IEnumerable<TodoItemDto>>(todoItems)).Returns(Enumerable.Empty<TodoItemDto>());

      // Act & Assert
      await Assert.ThrowsAsync<InvalidTodoItemMappingException>(() => _service.GetAllAsync());
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowInvalidTodoItemMappingException_WhenMappingFails()
    {
      // Arrange
      var createDto = new CreateTodoItemDto { Title = "Test" };
      _mockMapper.Setup(mapper => mapper.Map<TodoItem>(createDto)).Returns(new TodoItem { Title = "Test Item", UserId = "user123" });
      _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<TodoItem>())).ReturnsAsync(new TodoItem { Id = 1, Title = "Test Item", UserId = "user123" });
      _mockMapper.Setup(mapper => mapper.Map<TodoItemDto>(It.IsAny<TodoItem>())).Returns((TodoItemDto?)null);

      // Act & Assert
      await Assert.ThrowsAsync<InvalidTodoItemMappingException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowTodoItemOperationException_WhenRepositoryAddFails()
    {
      // Arrange
      var createDto = new CreateTodoItemDto { Title = "Test" };
      var todoItem = new TodoItem { Title = "Test", UserId = "user123" };
      _mockMapper.Setup(mapper => mapper.Map<TodoItem>(createDto)).Returns(todoItem);
      _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<TodoItem>())).ThrowsAsync(new Exception("DB error"));

      // Act & Assert
      await Assert.ThrowsAsync<TodoItemOperationException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowTodoItemNotFoundException_WhenItemDoesNotExist()
    {
      // Arrange
      var updateDto = new UpdateTodoItemDto { Title = "Updated" };
      _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((TodoItem?)null);

      // Act & Assert
      await Assert.ThrowsAsync<TodoItemNotFoundException>(() => _service.UpdateAsync(1, updateDto));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowTodoItemOperationException_WhenRepositoryUpdateFails()
    {
      // Arrange
      var existingItem = new TodoItem { Id = 1, Title = "Old", UserId = "user123" };
      var updateDto = new UpdateTodoItemDto { Title = "Updated" };
      _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingItem);
      _mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<TodoItem>())).ThrowsAsync(new Exception("DB error"));

      // Act & Assert
      await Assert.ThrowsAsync<TodoItemOperationException>(() => _service.UpdateAsync(1, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowTodoItemOperationException_WhenRepositoryDeleteFails()
    {
      // Arrange
      _mockRepo.Setup(repo => repo.DeleteAsync(1)).ThrowsAsync(new Exception("DB error"));

      // Act & Assert
      await Assert.ThrowsAsync<TodoItemOperationException>(() => _service.DeleteAsync(1));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldThrowInvalidTodoItemMappingException_WhenMappingFails()
    {
      // Arrange
      var userId = "testUser";
      var paginationRequestDto = new PaginationRequestDto { Limit = 2, Cursor = null };
      var todoItems = new List<TodoItem> { new TodoItem { Id = 1, Title = "Test Item", UserId = "user123" } };
      var domainPaginatedResult = new PaginatedResult<TodoItem>(todoItems, null);
      _mockRepo.Setup(repo => repo.GetPagedAsync(userId, It.IsAny<PaginationRequest>())).ReturnsAsync(domainPaginatedResult);
      _mockMapper.Setup(mapper => mapper.Map<List<TodoItemDto>>(It.IsAny<List<TodoItem>>())).Returns((List<TodoItemDto>?)null);

      // Act & Assert
      await Assert.ThrowsAsync<InvalidTodoItemMappingException>(() => _service.GetPagedAsync(userId, paginationRequestDto));
    }
  }
}