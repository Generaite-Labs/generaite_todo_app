using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using FluentAssertions;
using ToDo.Domain.Entities;
using ToDo.Domain.Interfaces;
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
    }
}