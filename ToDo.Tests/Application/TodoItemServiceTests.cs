using Moq;
using FluentAssertions;
using ToDo.Domain.Entities;
using ToDo.Domain.Interfaces;
using ToDo.Domain.Events;
using ToDo.Application.Services;
using ToDo.Application.DTOs;
using ToDo.Application.Exceptions;
using ToDo.Application.Interfaces;
using Microsoft.Extensions.Logging;
using ToDo.Domain.Common;
using AutoMapper;
using System.Linq;
using ToDo.Domain.ValueObjects; // Add this line to import TodoItemStatus

namespace ToDo.Tests.Application
{
  public class TodoItemServiceTests
  {
    private readonly Mock<ITodoItemRepository> _mockRepo;
    private readonly Mock<ILogger<TodoItemService>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IDomainEventDispatcher> _mockEventDispatcher;
    private readonly Mock<IDomainEventService> _mockDomainEventService;
    private readonly ITodoItemService _service;
    private const string ValidUserId = "user1";
    private const string InvalidUserId = "user2";

    public TodoItemServiceTests()
    {
      _mockRepo = new Mock<ITodoItemRepository>();
      _mockLogger = new Mock<ILogger<TodoItemService>>();
      _mockMapper = new Mock<IMapper>();
      _mockEventDispatcher = new Mock<IDomainEventDispatcher>();
      _mockDomainEventService = new Mock<IDomainEventService>();
      _service = new TodoItemService(
          _mockRepo.Object,
          _mockEventDispatcher.Object,
          _mockLogger.Object,
          _mockMapper.Object,
          _mockDomainEventService.Object
      );
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTodoItemDto_WhenItemExistsAndUserIsAuthorized()
    {
      // Arrange
      var todoItem = TodoItem.CreateTodoItem("Test Item", null, ValidUserId, null);
      var todoItemDto = new TodoItemDto { Id = 1, Title = "Test Item", UserId = ValidUserId };
      _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(todoItem);
      _mockMapper.Setup(mapper => mapper.Map<TodoItemDto>(todoItem)).Returns(todoItemDto);

      // Act
      var result = await _service.GetByIdAsync(ValidUserId, 1);

      // Assert
      result.Should().NotBeNull();
      result!.Id.Should().Be(1);
      result.Title.Should().Be("Test Item");
      result.UserId.Should().Be(ValidUserId);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowUnauthorizedTodoItemAccessException_WhenUserIsNotAuthorized()
    {
      // Arrange
      var todoItem = TodoItem.CreateTodoItem("Test Item", null, ValidUserId, null);
      _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(todoItem);

      // Act & Assert
      await Assert.ThrowsAsync<UnauthorizedTodoItemAccessException>(() => _service.GetByIdAsync(InvalidUserId, 1));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyUserOwnedItems()
    {
      // Arrange
      var todoItems = new List<TodoItem>
      {
          TodoItem.CreateTodoItem("Item 1", null, ValidUserId, null),
          TodoItem.CreateTodoItem("Item 2", null, ValidUserId, null)
      };
      var todoItemDtos = new List<TodoItemDto>
      {
          new TodoItemDto { Id = 1, Title = "Item 1", UserId = ValidUserId },
          new TodoItemDto { Id = 2, Title = "Item 2", UserId = ValidUserId }
      };
      _mockRepo.Setup(repo => repo.GetByUserIdAsync(ValidUserId)).ReturnsAsync(todoItems);
      _mockMapper.Setup(mapper => mapper.Map<IEnumerable<TodoItemDto>>(todoItems)).Returns(todoItemDtos);

      // Act
      var result = await _service.GetAllAsync(ValidUserId);

      // Assert
      result.Should().HaveCount(2);
      result.Should().Contain(dto => dto.Id == 1 && dto.Title == "Item 1" && dto.UserId == ValidUserId);
      result.Should().Contain(dto => dto.Id == 2 && dto.Title == "Item 2" && dto.UserId == ValidUserId);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnCreatedTodoItemDto()
    {
      // Arrange
      var createDto = new CreateTodoItemDto
      {
        Title = "New Item",
        Description = "Test Description",
        DueDate = DateTime.UtcNow.AddDays(1)
      };
      var createdItem = TodoItem.CreateTodoItem(createDto.Title, createDto.Description, ValidUserId, createDto.DueDate);
      var createdItemDto = new TodoItemDto
      {
        Id = 1,
        Title = "New Item",
        Description = "Test Description",
        UserId = ValidUserId,
        DueDate = createDto.DueDate,
        Status = TodoItemStatus.NotStarted, // Changed from TodoItemStatusDto.TODO
        CreatedAt = createdItem.CreatedAt,
        UpdatedAt = createdItem.UpdatedAt
      };
      _mockMapper.Setup(mapper => mapper.Map<TodoItem>(createDto)).Returns(createdItem);
      _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<TodoItem>())).ReturnsAsync(createdItem);
      _mockMapper.Setup(mapper => mapper.Map<TodoItemDto>(createdItem)).Returns(createdItemDto);

      // Act
      var result = await _service.CreateAsync(ValidUserId, createDto);

      // Assert
      Assert.NotNull(result);
      Assert.Equal(createdItemDto.Id, result.Id);
      Assert.Equal(createdItemDto.Title, result.Title);
      Assert.Equal(createdItemDto.Description, result.Description);
      Assert.Equal(createdItemDto.UserId, result.UserId);
      Assert.Equal(createdItemDto.DueDate, result.DueDate);
      Assert.Equal(createdItemDto.Status, result.Status);
      Assert.Equal(createdItemDto.CreatedAt, result.CreatedAt);
      Assert.Equal(createdItemDto.UpdatedAt, result.UpdatedAt);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnUpdatedTodoItemDto()
    {
      // Arrange
      var existingItem = TodoItem.CreateTodoItem("Old Title", "Old Description", ValidUserId, null);
      var updateDto = new UpdateTodoItemDto
      {
        Title = "Updated Title",
        Description = "Updated Description",
        DueDate = DateTime.UtcNow.AddDays(2)
      };
      var updatedItem = TodoItem.CreateTodoItem(updateDto.Title, updateDto.Description, ValidUserId, updateDto.DueDate);
      var updatedItemDto = new TodoItemDto
      {
        Id = 1,
        Title = "Updated Title",
        Description = "Updated Description",
        UserId = ValidUserId,
        DueDate = updateDto.DueDate,
        Status = TodoItemStatus.NotStarted, // Changed from TodoItemStatusDto.TODO
        UpdatedAt = DateTime.UtcNow
      };
      _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingItem);
      _mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<TodoItem>())).Returns(Task.CompletedTask);
      _mockMapper.Setup(mapper => mapper.Map<TodoItemDto>(It.IsAny<TodoItem>())).Returns(updatedItemDto);

      // Act
      var result = await _service.UpdateAsync(ValidUserId, 1, updateDto);

      // Assert
      result.Should().NotBeNull();
      result.Id.Should().Be(1);
      result.Title.Should().Be("Updated Title");
      result.Description.Should().Be("Updated Description");
      result.UserId.Should().Be(ValidUserId);
      result.DueDate.Should().Be(updateDto.DueDate);
      result.Status.Should().Be(TodoItemStatus.NotStarted); // Changed from TodoItemStatusDto.TODO
      result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task StartTodoItemAsync_ShouldUpdateItemStatusToInProgress()
    {
      // Arrange
      var todoItem = TodoItem.CreateTodoItem("Test Item", null, ValidUserId, null);
      _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(todoItem);

      // Act
      await _service.StartTodoItemAsync(ValidUserId, 1);

      // Assert
      todoItem.Status.Should().Be(TodoItemStatus.InProgress);
      todoItem.StartedAt.Should().NotBeNull();
      todoItem.StartedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
      _mockRepo.Verify(repo => repo.UpdateAsync(It.IsAny<TodoItem>()), Times.Once);
      _mockEventDispatcher.Verify(dispatcher => dispatcher.DispatchAsync(It.IsAny<DomainEvent>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task CompleteTodoItemAsync_ShouldUpdateItemStatusToCompleted()
    {
      // Arrange
      var todoItem = TodoItem.CreateTodoItem("Test Item", null, ValidUserId, null);
      _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(todoItem);

      // Act
      await _service.CompleteTodoItemAsync(ValidUserId, 1);

      // Assert
      todoItem.Status.Should().Be(TodoItemStatus.Completed);
      todoItem.CompletedAt.Should().NotBeNull();
      todoItem.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
      _mockRepo.Verify(repo => repo.UpdateAsync(It.IsAny<TodoItem>()), Times.Once);
      _mockEventDispatcher.Verify(dispatcher => dispatcher.DispatchAsync(It.IsAny<DomainEvent>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task AssignTodoItemAsync_ShouldUpdateAssignedUserId()
    {
      // Arrange
      var todoItem = TodoItem.CreateTodoItem("Test Item", null, ValidUserId, null);
      _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(todoItem);
      var newAssignedUserId = "newUser";

      // Act
      await _service.AssignTodoItemAsync(ValidUserId, 1, newAssignedUserId);

      // Assert
      todoItem.AssignedUserId.Should().Be(newAssignedUserId);
      _mockRepo.Verify(repo => repo.UpdateAsync(It.IsAny<TodoItem>()), Times.Once);
      _mockEventDispatcher.Verify(dispatcher => dispatcher.DispatchAsync(It.IsAny<DomainEvent>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallRepositoryDeleteMethod()
    {
      // Arrange
      var idToDelete = 1;
      var existingItem = TodoItem.CreateTodoItem("Test Item", null, ValidUserId, null);
      var idProperty = existingItem.GetType().GetProperty("Id");
      if (idProperty != null)
      {
          idProperty.SetValue(existingItem, idToDelete);
      }
      _mockRepo.Setup(repo => repo.GetByIdAsync(idToDelete)).ReturnsAsync(existingItem);

      // Act
      await _service.DeleteAsync(ValidUserId, idToDelete);

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
                TodoItem.CreateTodoItem("Task 1", null, userId, null),
                TodoItem.CreateTodoItem("Task 2", null, userId, null)
            };
      todoItems[0].GetType().GetProperty("Id")!.SetValue(todoItems[0], 1);
      todoItems[1].GetType().GetProperty("Id")!.SetValue(todoItems[1], 2);

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
                TodoItem.CreateTodoItem("Task 3", null, userId, null)
            };
      todoItems[0].GetType().GetProperty("Id")!.SetValue(todoItems[0], 3);

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
    public async Task GetAllAsync_ShouldThrowInvalidTodoItemMappingException_WhenMappingFails()
    {
      // Arrange
      var todoItems = new List<TodoItem> { TodoItem.CreateTodoItem("Test Item", null, "user123", null) };
      todoItems[0].GetType().GetProperty("Id")!.SetValue(todoItems[0], 1);
      _mockRepo.Setup(repo => repo.GetByUserIdAsync(ValidUserId)).ReturnsAsync(todoItems);
      _mockMapper.Setup(mapper => mapper.Map<IEnumerable<TodoItemDto>>(todoItems)).Returns(Enumerable.Empty<TodoItemDto>());

      // Act & Assert
      await Assert.ThrowsAsync<InvalidTodoItemMappingException>(() => _service.GetAllAsync(ValidUserId));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowInvalidTodoItemMappingException_WhenMappingFails()
    {
      // Arrange
      var createDto = new CreateTodoItemDto { Title = "Test Todo" };
      _mockMapper.Setup(m => m.Map<TodoItem>(createDto)).Returns(TodoItem.CreateTodoItem("Test Todo", null, "testUserId", null));

      // Act & Assert
      await Assert.ThrowsAsync<InvalidTodoItemMappingException>(() => _service.CreateAsync(ValidUserId, createDto));
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowTodoItemOperationException_WhenRepositoryAddFails()
    {
      // Arrange
      var createDto = new CreateTodoItemDto { Title = "Test" };
      var todoItem = TodoItem.CreateTodoItem("Test", null, ValidUserId, null);
      _mockMapper.Setup(m => m.Map<TodoItem>(createDto)).Returns(todoItem);
      _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<TodoItem>())).ThrowsAsync(new Exception("DB error"));

      // Act & Assert
      var exception = await Assert.ThrowsAsync<TodoItemOperationException>(() => _service.CreateAsync(ValidUserId, createDto));
      Assert.Equal("Create", exception.Operation);
      Assert.Contains("DB error", exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowTodoItemNotFoundException_WhenItemDoesNotExist()
    {
      // Arrange
      var updateDto = new UpdateTodoItemDto { Title = "Updated" };
      _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((TodoItem?)null);

      // Act & Assert
      await Assert.ThrowsAsync<UnauthorizedTodoItemAccessException>(() => _service.UpdateAsync(ValidUserId, 1, updateDto));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowTodoItemOperationException_WhenRepositoryUpdateFails()
    {
      // Arrange
      var existingItem = TodoItem.CreateTodoItem("Old Title", null, ValidUserId, null);
      var updateDto = new UpdateTodoItemDto { Title = "Updated" };
      _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingItem);
      _mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<TodoItem>())).ThrowsAsync(new Exception("DB error"));

      // Act & Assert
      await Assert.ThrowsAsync<TodoItemOperationException>(() => _service.UpdateAsync(ValidUserId, 1, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowTodoItemOperationException_WhenRepositoryDeleteFails()
    {
      // Arrange
      var existingItem = TodoItem.CreateTodoItem("Test Item", null, ValidUserId, null);
      _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingItem);
      _mockRepo.Setup(repo => repo.DeleteAsync(1)).ThrowsAsync(new Exception("DB error"));

      // Act & Assert
      await Assert.ThrowsAsync<TodoItemOperationException>(() => _service.DeleteAsync(ValidUserId, 1));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldThrowInvalidTodoItemMappingException_WhenMappingFails()
    {
      // Arrange
      var userId = "testUser";
      var paginationRequestDto = new PaginationRequestDto { Limit = 2, Cursor = null };
      var todoItems = new List<TodoItem> { TodoItem.CreateTodoItem("Test Item", null, "user123", null) };
      var domainPaginatedResult = new PaginatedResult<TodoItem>(todoItems, null);
      _mockRepo.Setup(repo => repo.GetPagedAsync(userId, It.IsAny<PaginationRequest>())).ReturnsAsync(domainPaginatedResult);
      _mockMapper.Setup(mapper => mapper.Map<List<TodoItemDto>>(It.IsAny<List<TodoItem>>())).Returns((List<TodoItemDto>?)null);

      // Act & Assert
      await Assert.ThrowsAsync<InvalidTodoItemMappingException>(() => _service.GetPagedAsync(userId, paginationRequestDto));
    }
  }
}