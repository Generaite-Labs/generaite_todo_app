using Moq;
using FluentAssertions;
using ToDo.Domain.Entities;
using ToDo.Domain.Interfaces;
using ToDo.Application.Services;
using ToDo.Core.DTOs;
using ToDo.Application.Exceptions;
using ToDo.Application.Interfaces;
using Microsoft.Extensions.Logging;
using ToDo.Domain.Common;
using AutoMapper;
using ToDo.Domain.ValueObjects;

namespace ToDo.Tests.Application
{
  public class TodoItemServiceTests
  {
    private readonly Mock<ITodoItemRepository> _mockRepo;
    private readonly Mock<ILogger<TodoItemService>> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly ITodoItemService _service;
    private const string ValidUserId = "user1";
    private const string InvalidUserId = "user2";

    public TodoItemServiceTests()
    {
      _mockRepo = new Mock<ITodoItemRepository>();
      _mockLogger = new Mock<ILogger<TodoItemService>>();
      _mockMapper = new Mock<IMapper>();
      _mockUnitOfWork = new Mock<IUnitOfWork>();
      _service = new TodoItemService(
          _mockRepo.Object,
          _mockLogger.Object,
          _mockMapper.Object,
          _mockUnitOfWork.Object
      );
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTodoItemDto_WhenItemExistsAndUserIsAuthorized()
    {
      // Arrange
      var itemId = Guid.NewGuid();
      var todoItem = TodoItem.CreateTodoItem("Test Item", null, ValidUserId, null);
      var todoItemDto = new TodoItemDto { Id = itemId, Title = "Test Item", UserId = ValidUserId };
      _mockRepo.Setup(repo => repo.GetByIdAsync(itemId)).ReturnsAsync(todoItem);
      _mockMapper.Setup(mapper => mapper.Map<TodoItemDto>(todoItem)).Returns(todoItemDto);

      // Act
      var result = await _service.GetByIdAsync(ValidUserId, itemId);

      // Assert
      result.Should().NotBeNull();
      result!.Id.Should().Be(itemId);
      result.Title.Should().Be("Test Item");
      result.UserId.Should().Be(ValidUserId);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowUnauthorizedTodoItemAccessException_WhenUserIsNotAuthorized()
    {
      // Arrange
      var itemId = Guid.NewGuid();
      var todoItem = TodoItem.CreateTodoItem("Test Item", null, ValidUserId, null);
      _mockRepo.Setup(repo => repo.GetByIdAsync(itemId)).ReturnsAsync(todoItem);

      // Act & Assert
      await Assert.ThrowsAsync<UnauthorizedTodoItemAccessException>(() => 
          _service.GetByIdAsync(InvalidUserId, itemId));
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
          new TodoItemDto { Id = Guid.NewGuid(), Title = "Item 1", UserId = ValidUserId },
          new TodoItemDto { Id = Guid.NewGuid(), Title = "Item 2", UserId = ValidUserId }
      };
      _mockRepo.Setup(repo => repo.GetByUserIdAsync(ValidUserId)).ReturnsAsync(todoItems);
      _mockMapper.Setup(mapper => mapper.Map<IEnumerable<TodoItemDto>>(todoItems)).Returns(todoItemDtos);

      // Act
      var result = await _service.GetAllAsync(ValidUserId);

      // Assert
      result.Should().HaveCount(2);
      result.Should().Contain(dto => dto.Title == "Item 1" && dto.UserId == ValidUserId);
      result.Should().Contain(dto => dto.Title == "Item 2" && dto.UserId == ValidUserId);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnCreatedTodoItemDto()
    {
      // Arrange
      var itemId = Guid.NewGuid();
      var createDto = new CreateTodoItemDto
      {
        Title = "New Item",
        Description = "Test Description",
        DueDate = DateTime.UtcNow.AddDays(1)
      };
      
      var todoItem = TodoItem.CreateTodoItem(
          createDto.Title,
          createDto.Description,
          ValidUserId,
          createDto.DueDate
      );
      
      var createdItemDto = new TodoItemDto
      {
        Id = itemId,
        Title = "New Item",
        Description = "Test Description",
        UserId = ValidUserId,
        DueDate = createDto.DueDate,
        Status = TodoItemStatus.NotStarted.ToString(),
        CreatedAt = todoItem.CreatedAt,
        UpdatedAt = todoItem.UpdatedAt
      };

      // Set up repository to return the created item
      _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<TodoItem>()))
              .ReturnsAsync(todoItem);

      // Set up mapper to handle any TodoItem to TodoItemDto mapping
      _mockMapper.Setup(mapper => mapper.Map<TodoItemDto>(It.IsAny<TodoItem>()))
              .Returns(createdItemDto);

      // Set up UnitOfWork to succeed
      _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
              .ReturnsAsync(true);

      // Act
      var result = await _service.CreateAsync(ValidUserId, createDto);

      // Assert
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(createdItemDto);
      
      // Verify interactions
      _mockRepo.Verify(repo => repo.AddAsync(It.Is<TodoItem>(item => 
          item.Title == createDto.Title && 
          item.Description == createDto.Description && 
          item.UserId == ValidUserId)), 
          Times.Once);
      _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
      _mockMapper.Verify(mapper => mapper.Map<TodoItemDto>(It.IsAny<TodoItem>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnUpdatedTodoItemDto()
    {
      // Arrange
      var itemId = Guid.NewGuid();
      var existingItem = TodoItem.CreateTodoItem("Old Title", "Old Description", ValidUserId, null);
      var updateDto = new UpdateTodoItemDto
      {
        Title = "Updated Title",
        Description = "Updated Description",
        DueDate = DateTime.UtcNow.AddDays(2)
      };
      var updatedItemDto = new TodoItemDto
      {
        Id = itemId,
        Title = "Updated Title",
        Description = "Updated Description",
        UserId = ValidUserId,
        DueDate = updateDto.DueDate,
        Status = TodoItemStatus.NotStarted.ToString(),
        UpdatedAt = DateTime.UtcNow
      };

      _mockRepo.Setup(repo => repo.GetByIdAsync(itemId)).ReturnsAsync(existingItem);
      _mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<TodoItem>())).Returns(Task.CompletedTask);
      _mockMapper.Setup(mapper => mapper.Map<TodoItemDto>(It.IsAny<TodoItem>())).Returns(updatedItemDto);
      _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

      // Act
      var result = await _service.UpdateAsync(ValidUserId, itemId, updateDto);

      // Assert
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(updatedItemDto);
      _mockRepo.Verify(repo => repo.UpdateAsync(It.IsAny<TodoItem>()), Times.Once);
      _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StartTodoItemAsync_ShouldUpdateItemStatusAndSaveChanges()
    {
      // Arrange
      var itemId = Guid.NewGuid();
      var todoItem = TodoItem.CreateTodoItem("Test Item", null, ValidUserId, null);
      _mockRepo.Setup(repo => repo.GetByIdAsync(itemId)).ReturnsAsync(todoItem);
      _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

      // Act
      await _service.StartTodoItemAsync(ValidUserId, itemId);

      // Assert
      todoItem.Status.Should().Be(TodoItemStatus.InProgress);
      _mockRepo.Verify(repo => repo.UpdateAsync(It.IsAny<TodoItem>()), Times.Once);
      _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CompleteTodoItemAsync_ShouldUpdateItemStatusToCompleted()
    {
      // Arrange
      var itemId = Guid.NewGuid();
      var todoItem = TodoItem.CreateTodoItem("Test Item", null, ValidUserId, null);
      _mockRepo.Setup(repo => repo.GetByIdAsync(itemId)).ReturnsAsync(todoItem);

      // Act
      await _service.CompleteTodoItemAsync(ValidUserId, itemId);

      // Assert
      todoItem.Status.Should().Be(TodoItemStatus.Completed);
      todoItem.CompletedAt.Should().NotBeNull();
      todoItem.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
      _mockRepo.Verify(repo => repo.UpdateAsync(It.IsAny<TodoItem>()), Times.Once);
    }

    [Fact]
    public async Task AssignTodoItemAsync_ShouldUpdateAssignedUserId()
    {
      // Arrange
      var itemId = Guid.NewGuid();
      var todoItem = TodoItem.CreateTodoItem("Test Item", null, ValidUserId, null);
      _mockRepo.Setup(repo => repo.GetByIdAsync(itemId)).ReturnsAsync(todoItem);
      var newAssignedUserId = "newUser";

      // Act
      await _service.AssignTodoItemAsync(ValidUserId, itemId, newAssignedUserId);

      // Assert
      todoItem.AssignedUserId.Should().Be(newAssignedUserId);
      _mockRepo.Verify(repo => repo.UpdateAsync(It.IsAny<TodoItem>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallRepositoryDeleteMethodAndUnitOfWork()
    {
      // Arrange
      var idToDelete = Guid.NewGuid();
      var existingItem = TodoItem.CreateTodoItem("Test Item", null, ValidUserId, null);
      var idProperty = existingItem.GetType().GetProperty("Id");
      if (idProperty != null)
      {
          idProperty.SetValue(existingItem, idToDelete);
      }
      _mockRepo.Setup(repo => repo.GetByIdAsync(idToDelete)).ReturnsAsync(existingItem);
      _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

      // Act
      await _service.DeleteAsync(ValidUserId, idToDelete);

      // Assert
      _mockRepo.Verify(repo => repo.DeleteAsync(idToDelete), Times.Once);
      _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
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
      var id1 = Guid.NewGuid();
      var id2 = Guid.NewGuid();
      todoItems[0].GetType().GetProperty("Id")!.SetValue(todoItems[0], id1);
      todoItems[1].GetType().GetProperty("Id")!.SetValue(todoItems[1], id2);

      var todoItemDtos = new List<TodoItemDto>
            {
                new TodoItemDto { Id = id1, Title = "Task 1", UserId = userId },
                new TodoItemDto { Id = id2, Title = "Task 2", UserId = userId }
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
      var id3 = Guid.NewGuid();
      todoItems[0].GetType().GetProperty("Id")!.SetValue(todoItems[0], id3);

      var todoItemDtos = new List<TodoItemDto>
            {
                new TodoItemDto { Id = id3, Title = "Task 3", UserId = userId }
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
      var todoItem = TodoItem.CreateTodoItem("Test Item", "Description", ValidUserId, DateTime.UtcNow.AddDays(1));
      var todoItems = new List<TodoItem> { todoItem };
      
      _mockRepo.Setup(repo => repo.GetByUserIdAsync(ValidUserId)).ReturnsAsync(todoItems);
      
      // Setup mapper to return null, simulating a mapping failure
      _mockMapper.Setup(mapper => mapper.Map<IEnumerable<TodoItemDto>>(It.IsAny<IEnumerable<TodoItem>>()))
                 .Returns(Enumerable.Empty<TodoItemDto>());

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
      _mockRepo.Setup(repo => repo.GetByIdAsync(Guid.NewGuid())).ReturnsAsync((TodoItem?)null);

      // Act & Assert
      await Assert.ThrowsAsync<UnauthorizedTodoItemAccessException>(() => _service.UpdateAsync(ValidUserId, Guid.NewGuid(), updateDto));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowTodoItemOperationException_WhenRepositoryUpdateFails()
    {
      // Arrange
      var itemId = Guid.NewGuid();
      var existingItem = TodoItem.CreateTodoItem("Old Title", null, ValidUserId, null);
      // Set the ID on the existing item
      var idProperty = existingItem.GetType().GetProperty("Id");
      idProperty?.SetValue(existingItem, itemId);
      
      var updateDto = new UpdateTodoItemDto { Title = "Updated" };
      
      _mockRepo.Setup(repo => repo.GetByIdAsync(itemId)).ReturnsAsync(existingItem);
      _mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<TodoItem>()))
          .ThrowsAsync(new Exception("DB error"));

      // Act & Assert
      var exception = await Assert.ThrowsAsync<TodoItemOperationException>(
          () => _service.UpdateAsync(ValidUserId, itemId, updateDto));
      
      Assert.Equal("Update", exception.Operation);
      Assert.Contains("DB error", exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowTodoItemOperationException_WhenRepositoryDeleteFails()
    {
      // Arrange
      var itemId = Guid.NewGuid();
      var existingItem = TodoItem.CreateTodoItem("Test Item", null, ValidUserId, null);
      var idProperty = existingItem.GetType().GetProperty("Id");
      idProperty?.SetValue(existingItem, itemId);
      
      _mockRepo.Setup(repo => repo.GetByIdAsync(itemId)).ReturnsAsync(existingItem);
      _mockRepo.Setup(repo => repo.DeleteAsync(itemId))
          .ThrowsAsync(new Exception("Failed to delete item"));

      // Act & Assert
      var exception = await Assert.ThrowsAsync<TodoItemOperationException>(
          () => _service.DeleteAsync(ValidUserId, itemId));
      
      Assert.Equal("Delete", exception.Operation);
      Assert.Contains("'Delete' failed", exception.Message);
      
      // Verify the delete was attempted
      _mockRepo.Verify(repo => repo.DeleteAsync(itemId), Times.Once);
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

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenRepositoryReturnsEmptyList()
    {
      // Arrange
      var emptyList = new List<TodoItem>();
      _mockRepo.Setup(repo => repo.GetByUserIdAsync(ValidUserId)).ReturnsAsync(emptyList);
      _mockMapper.Setup(mapper => mapper.Map<IEnumerable<TodoItemDto>>(It.IsAny<IEnumerable<TodoItem>>()))
                 .Returns(Enumerable.Empty<TodoItemDto>());

      // Act
      var result = await _service.GetAllAsync(ValidUserId);

      // Assert
      result.Should().NotBeNull();
      result.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowWhenUnitOfWorkFails()
    {
      // Arrange
      var createDto = new CreateTodoItemDto { Title = "Test" };
      var todoItem = TodoItem.CreateTodoItem("Test", null, ValidUserId, null);
      _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<TodoItem>())).ReturnsAsync(todoItem);
      _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new Exception("Database error"));

      // Act & Assert
      var exception = await Assert.ThrowsAsync<TodoItemOperationException>(
          () => _service.CreateAsync(ValidUserId, createDto));
      exception.Operation.Should().Be("Create");
      exception.Message.Should().Contain("Database error");
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowWhenUnitOfWorkFails()
    {
      // Arrange
      var itemId = Guid.NewGuid();
      var existingItem = TodoItem.CreateTodoItem("Test", null, ValidUserId, null);
      // Set the ID on the existing item
      var idProperty = existingItem.GetType().GetProperty("Id");
      idProperty?.SetValue(existingItem, itemId);
      
      var updateDto = new UpdateTodoItemDto { Title = "Updated" };
      
      _mockRepo.Setup(repo => repo.GetByIdAsync(itemId)).ReturnsAsync(existingItem);
      _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new Exception("Database error"));

      // Act & Assert
      var exception = await Assert.ThrowsAsync<TodoItemOperationException>(
          () => _service.UpdateAsync(ValidUserId, itemId, updateDto));
        
      exception.Operation.Should().Be("Update");
      exception.Message.Should().Contain("Database error");
    }
  }
}
