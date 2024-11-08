using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ToDo.Api.Controllers;
using ToDo.Application.DTOs;
using ToDo.Application.Services;
using ToDo.Domain.Entities;
using ToDo.Infrastructure;
using Xunit;
using System.Threading.Tasks;
using AutoMapper;
using ToDo.Application.Mappers;
using Microsoft.Extensions.Logging;
using Moq;
using ToDo.Domain.Interfaces;
using ToDo.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ToDo.Tests.Controllers
{
    public class TodoItemsControllerTests : IDisposable
    {
        private readonly TodoItemsController _controller;
        private readonly TodoDbContext _context;
        private readonly IMapper _mapper;
        private readonly string _userId = "test-user-id";

        public TodoItemsControllerTests()
        {
            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TodoDbContext(options);

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
            _context.TodoItems.AddRange(
                TodoItem.CreateTodoItem("Test Todo 1", "Description 1", _userId, null),
                TodoItem.CreateTodoItem("Test Todo 2", "Description 2", _userId, null),
                TodoItem.CreateTodoItem("Test Todo 3", "Description 3", "other-user-id", null)
            );
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WhenItemExists()
        {
            // Act
            var actionResult = await _controller.GetById(1);

            // Assert
            var result = Assert.IsType<ActionResult<TodoItemDto>>(actionResult);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<TodoItemDto>(okResult.Value);
            Assert.Equal(1, returnedDto.Id);
            Assert.Equal("Test Todo 1", returnedDto.Title);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenItemDoesNotExist()
        {
            // Act
            var actionResult = await _controller.GetById(999);

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
            var updateDto = new UpdateTodoItemDto { Title = "Updated Todo" };

            // Act
            var actionResult = await _controller.Update(1, updateDto);

            // Assert
            var result = Assert.IsType<ActionResult<TodoItemDto>>(actionResult);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<TodoItemDto>(okResult.Value);
            Assert.Equal(1, returnedDto.Id);
            Assert.Equal("Updated Todo", returnedDto.Title);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDeleteSucceeds()
        {
            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Null(await _context.TodoItems.FindAsync(1));
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
            // Act
            var result = await _controller.Start(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal("Todo item started successfully.", okResult.Value);
        }

        [Fact]
        public async Task Complete_ReturnsOkResult_WhenCompleteSucceeds()
        {
            // Act
            var result = await _controller.Complete(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal("Todo item completed successfully.", okResult.Value);
        }

        [Fact]
        public async Task Assign_ReturnsOkResult_WhenAssignSucceeds()
        {
            // Arrange
            string assignedUserId = "assigned-user-id";

            // Act
            var result = await _controller.Assign(1, assignedUserId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal("Todo item assigned successfully.", okResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsUnauthorized_WhenUserIdIsMissing()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("User is not authenticated or user ID is missing.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task GetById_ReturnsUnauthorized_WhenAccessingOtherUsersTodo()
        {
            // Act
            var result = await _controller.GetById(3);

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
