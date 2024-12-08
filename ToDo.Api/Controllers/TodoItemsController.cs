using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDo.Core.DTOs;
using ToDo.Application.Interfaces;
using System.Security.Claims;
using ToDo.Application.Exceptions;
using ToDo.Domain.Common;

namespace ToDo.Api.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  [Authorize]
  public class TodoItemsController : ControllerBase
  {
    private readonly ITodoItemService _todoItemService;

    public TodoItemsController(ITodoItemService todoItemService)
    {
      _todoItemService = todoItemService ?? throw new ArgumentNullException(nameof(todoItemService));
    }

    private ActionResult HandleException(Exception ex)
    {
      return ex switch
      {
        UnauthorizedAccessException or UnauthorizedTodoItemAccessException => Unauthorized(ex.Message),
        TodoItemNotFoundException => NotFound(ex.Message),
        _ => StatusCode(500, "An unexpected error occurred. Please try again later.")
      };
    }

    private async Task<ActionResult<T>> ExecuteServiceMethod<T>(Func<string, Task<T>> serviceMethod)
    {
      try
      {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
          return Unauthorized("User is not authenticated or user ID is missing.");
        }
        var result = await serviceMethod(userId);
        if (result == null)
        {
          return NotFound("Todo item not found.");
        }
        return Ok(result);
      }
      catch (Exception ex)
      {
        return HandleException(ex);
      }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TodoItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<ActionResult<TodoItemDto?>> GetById(Guid id) =>
        ExecuteServiceMethod(userId => _todoItemService.GetByIdAsync(userId, id));

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TodoItemDto>), StatusCodes.Status200OK)]
    public Task<ActionResult<IEnumerable<TodoItemDto>>> GetAll() =>
        ExecuteServiceMethod(_todoItemService.GetAllAsync);

    [HttpPost]
    [ProducesResponseType(typeof(TodoItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TodoItemDto>> Create(CreateTodoItemDto createDto)
    {
      try
      {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
          return Unauthorized("User is not authenticated or user ID is missing.");
        }
        var createdItem = await _todoItemService.CreateAsync(userId, createDto);
        return CreatedAtAction(nameof(GetById), new { id = createdItem.Id }, createdItem);
      }
      catch (Exception ex)
      {
        return HandleException(ex);
      }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TodoItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<ActionResult<TodoItemDto>> Update(Guid id, UpdateTodoItemDto updateDto) =>
        ExecuteServiceMethod(userId => _todoItemService.UpdateAsync(userId, id, updateDto));

    [HttpPost("{id}/start")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<ActionResult<string>> Start(Guid id) =>
        ExecuteServiceMethod(async userId =>
        {
          await _todoItemService.StartTodoItemAsync(userId, id);
          return "Todo item started successfully.";
        });

    [HttpPost("{id}/stop")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<ActionResult<string>> Stop(Guid id) =>
        ExecuteServiceMethod(async userId =>
        {
          await _todoItemService.StopTodoItemAsync(userId, id);
          return "Todo item stopped successfully.";
        });

    [HttpPost("{id}/complete")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<ActionResult<string>> Complete(Guid id) =>
        ExecuteServiceMethod(async userId =>
        {
          await _todoItemService.CompleteTodoItemAsync(userId, id);
          return "Todo item completed successfully.";
        });

    [HttpPost("{id}/assign")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<ActionResult<string>> Assign(Guid id, [FromBody] string assignedUserId) =>
        ExecuteServiceMethod(async userId =>
        {
          await _todoItemService.AssignTodoItemAsync(userId, id, assignedUserId);
          return "Todo item assigned successfully.";
        });

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id)
    {
      await ExecuteServiceMethod(async userId =>
      {
        await _todoItemService.DeleteAsync(userId, id);
        return new object(); // Return a non-null object
      });
      return NoContent();
    }

    [HttpGet("paged")]
    [ProducesResponseType(typeof(PaginatedResultDto<TodoItemDto>), StatusCodes.Status200OK)]
    public Task<ActionResult<PaginatedResultDto<TodoItemDto>>> GetPaged([FromQuery] PaginationRequestDto paginationRequestDto) =>
        ExecuteServiceMethod(userId => _todoItemService.GetPagedAsync(userId, paginationRequestDto));
  }
}
