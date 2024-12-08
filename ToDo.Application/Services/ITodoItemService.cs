using ToDo.Core.DTOs;

namespace ToDo.Application.Interfaces
{
  public interface ITodoItemService
  {
    Task<TodoItemDto?> GetByIdAsync(string userId, Guid id);
    Task<IEnumerable<TodoItemDto>> GetAllAsync(string userId);
    Task<TodoItemDto> CreateAsync(string userId, CreateTodoItemDto createDto);
    Task<TodoItemDto> UpdateAsync(string userId, Guid id, UpdateTodoItemDto updateDto);
    Task StartTodoItemAsync(string userId, Guid id);
    Task StopTodoItemAsync(string userId, Guid id);
    Task CompleteTodoItemAsync(string userId, Guid id);
    Task AssignTodoItemAsync(string userId, Guid id, string assignedUserId);
    Task DeleteAsync(string userId, Guid id);
    Task<PaginatedResultDto<TodoItemDto>> GetPagedAsync(string userId, PaginationRequestDto paginationRequestDto);
  }
}
