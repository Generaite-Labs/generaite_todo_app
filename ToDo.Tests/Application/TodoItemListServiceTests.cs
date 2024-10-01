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
    public class TodoItemListServiceTests
    {
        private readonly Mock<ITodoItemListRepository> _mockRepo;
        private readonly ITodoItemListService _service;

        public TodoItemListServiceTests()
        {
            _mockRepo = new Mock<ITodoItemListRepository>();
            _service = new TodoItemListService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnTodoItemListDto_WhenListExists()
        {
            // Arrange
            var todoItemList = new TodoItemList { Id = 1, Name = "Test List", UserId = "user1" };
            _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(todoItemList);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Name.Should().Be("Test List");
            result.UserId.Should().Be("user1");
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllListDtos()
        {
            // Arrange
            var todoItemLists = new List<TodoItemList>
            {
                new TodoItemList { Id = 1, Name = "List 1", UserId = "user1" },
                new TodoItemList { Id = 2, Name = "List 2", UserId = "user2" }
            };
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(todoItemLists);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(dto => dto.Id == 1 && dto.Name == "List 1" && dto.UserId == "user1");
            result.Should().Contain(dto => dto.Id == 2 && dto.Name == "List 2" && dto.UserId == "user2");
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnUserListDtos()
        {
            // Arrange
            var userId = "user123";
            var todoItemLists = new List<TodoItemList>
            {
                new TodoItemList { Id = 1, Name = "User List 1", UserId = userId },
                new TodoItemList { Id = 2, Name = "User List 2", UserId = userId }
            };
            _mockRepo.Setup(repo => repo.GetByUserIdAsync(userId)).ReturnsAsync(todoItemLists);

            // Act
            var result = await _service.GetByUserIdAsync(userId);

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(dto => dto.UserId == userId);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnCreatedTodoItemListDto()
        {
            // Arrange
            var createDto = new CreateTodoItemListDto { Name = "New List", UserId = "user1" };
            _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<TodoItemList>()))
                .ReturnsAsync((TodoItemList list) => list);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("New List");
            result.UserId.Should().Be("user1");
            result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            _mockRepo.Verify(repo => repo.AddAsync(It.Is<TodoItemList>(l => l.Name == "New List" && l.UserId == "user1")), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnUpdatedTodoItemListDto()
        {
            // Arrange
            var updateDto = new UpdateTodoItemListDto { Id = 1, Name = "Updated List" };
            var existingList = new TodoItemList { Id = 1, Name = "Old List", UserId = "user1" };
            _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingList);

            // Act
            var result = await _service.UpdateAsync(updateDto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Updated List");
            result.UserId.Should().Be("user1");
            result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            _mockRepo.Verify(repo => repo.UpdateAsync(It.Is<TodoItemList>(l => l.Id == 1 && l.Name == "Updated List")), Times.Once);
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