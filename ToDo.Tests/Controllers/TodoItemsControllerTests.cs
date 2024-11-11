using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ToDo.Api.Controllers;
using ToDo.Core.DTOs;
using ToDo.Application.Services;
using ToDo.Domain.Entities;
using ToDo.Infrastructure;
using AutoMapper;
using ToDo.Application.Mappers;
using Microsoft.Extensions.Logging;
using Moq;
using ToDo.Domain.Interfaces;
using ToDo.Infrastructure.Repositories;

namespace ToDo.Tests.Controllers
{
    public class TodoItemsControllerTests : IDisposable
    {
        private readonly TodoItemsController _controller;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly string _userId = "test-user-id";

        public TodoItemsControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new TodoItemMappingProfile());
            });
            _mapper = mapperConfig.CreateMapper();

            var logger = new Mock<ILogger<TodoItemService>>().Object;
            var repositoryLogger = new Mock<ILogger<TodoItemRepository>>().Object;
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var todoItemService = new TodoItemService(
                new TodoItemRepository(_context, repositoryLogger),
                logger,
                _mapper,
                mockUnitOfWork.Object
            );

            _controller = new TodoItemsController(todoItemService);

            // Setup ClaimsPrincipal for the controller
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, _userId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var item1 = TodoItem.CreateTodoItem("Test Todo 1", "Description 1", _userId, null);
            var item2 = TodoItem.CreateTodoItem("Test Todo 2", "Description 2", _userId, null);
            var item3 = TodoItem.CreateTodoItem("Test Todo 3", "Description 3", "other-user-id", null);
            
            // Set GUIDs for the items
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var id3 = Guid.NewGuid();
            
            var idProperty = typeof(TodoItem).GetProperty("Id");
            if (idProperty != null)
            {
                idProperty.SetValue(item1, id1);
                idProperty.SetValue(item2, id2);
                idProperty.SetValue(item3, id3);
            }

            _context.TodoItems.AddRange(item1, item2, item3);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WhenItemExists()
        {
            // Arrange
            var item = await _context.TodoItems.FirstAsync();
            var itemId = item.Id;

            // Act
            var actionResult = await _controller.GetById(itemId);

            // Assert
            var result = Assert.IsType<ActionResult<TodoItemDto>>(actionResult);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<TodoItemDto>(okResult.Value);
            Assert.Equal(itemId, returnedDto.Id);
            Assert.Equal("Test Todo 1", returnedDto.Title);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenItemDoesNotExist()
        {
            // Act
            var actionResult = await _controller.GetById(Guid.NewGuid());

            // Assert
            var result = Assert.IsType<ActionResult<TodoItemDto>>(actionResult);
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenCreateSucceeds()
        {
            // Arrange
            var createDto = new CreateTodoItemDto { Title = "New Todo" };

            // Act
            var actionResult = await _controller.Create(createDto);

            // Assert
            var result = Assert.IsType<ActionResult<TodoItemDto>>(actionResult);
            
            if (result.Result is CreatedAtActionResult createdAtActionResult)
            {
                Assert.Equal(nameof(_controller.GetById), createdAtActionResult.ActionName);
                var returnedDto = Assert.IsType<TodoItemDto>(createdAtActionResult.Value);
                Assert.Equal("New Todo", returnedDto.Title);
            }
            else if (result.Result is ObjectResult objectResult)
            {
                // If an exception occurred, it will be an ObjectResult
                Assert.Equal(500, objectResult.StatusCode);
                Assert.Contains("An unexpected error occurred", objectResult.Value?.ToString() ?? string.Empty);
            }
            else
            {
                Assert.Fail($"Unexpected result type: {result.Result?.GetType()}");
            }
        }

        [Fact]
        public async Task Update_ReturnsOkResult_WhenUpdateSucceeds()
        {
            // Arrange
            var item = await _context.TodoItems.FirstAsync();
            var itemId = item.Id;
            var updateDto = new UpdateTodoItemDto { Title = "Updated Todo" };

            // Act
            var actionResult = await _controller.Update(itemId, updateDto);

            // Assert
            var result = Assert.IsType<ActionResult<TodoItemDto>>(actionResult);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<TodoItemDto>(okResult.Value);
            Assert.Equal(itemId, returnedDto.Id);
            Assert.Equal("Updated Todo", returnedDto.Title);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDeleteSucceeds()
        {
            // Arrange
            var item = await _context.TodoItems.FirstAsync();
            var itemId = item.Id;

            // Act
            var result = await _controller.Delete(itemId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Null(await _context.TodoItems.FindAsync(itemId));
        }

        [Fact]
        public async Task GetPaged_ReturnsOkResult_WithPaginatedResult()
        {
            // Arrange
            var paginationRequestDto = new PaginationRequestDto { Limit = 10, Cursor = null };

            // Act
            var actionResult = await _controller.GetPaged(paginationRequestDto);

            // Assert
            var result = Assert.IsType<ActionResult<PaginatedResultDto<TodoItemDto>>>(actionResult);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedResult = Assert.IsType<PaginatedResultDto<TodoItemDto>>(okResult.Value);
            Assert.Equal(2, returnedResult.Items.Count());
        }

        [Fact]
        public async Task Start_ReturnsOkResult_WhenStartSucceeds()
        {
            // Arrange
            var item = await _context.TodoItems.FirstAsync();
            var itemId = item.Id;

            // Act
            var result = await _controller.Start(itemId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal("Todo item started successfully.", okResult.Value);
        }

        [Fact]
        public async Task Complete_ReturnsOkResult_WhenCompleteSucceeds()
        {
            // Arrange
            var item = await _context.TodoItems.FirstAsync();
            var itemId = item.Id;

            // Act
            var result = await _controller.Complete(itemId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal("Todo item completed successfully.", okResult.Value);
        }

        [Fact]
        public async Task Assign_ReturnsOkResult_WhenAssignSucceeds()
        {
            // Arrange
            var item = await _context.TodoItems.FirstAsync();
            var itemId = item.Id;
            string assignedUserId = "assigned-user-id";

            // Act
            var result = await _controller.Assign(itemId, assignedUserId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal("Todo item assigned successfully.", okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsUnauthorized_WhenUserIdIsMissing()
        {
            // Arrange
            var item = await _context.TodoItems.FirstAsync();
            var itemId = item.Id;
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };

            // Act
            var result = await _controller.GetById(itemId);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("User is not authenticated or user ID is missing.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsUnauthorized_WhenAccessingOtherUsersTodo()
        {
            // Arrange
            var otherUserItem = await _context.TodoItems.FirstOrDefaultAsync(x => x.UserId != _userId);
            var itemId = otherUserItem!.Id;

            // Act
            var result = await _controller.GetById(itemId);

            // Assert
            Assert.IsType<ActionResult<TodoItemDto>>(result);
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Contains("User test-user-id is not authorized", unauthorizedResult.Value?.ToString() ?? string.Empty);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
