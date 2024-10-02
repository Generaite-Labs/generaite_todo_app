using ToDo.Application.DTOs;

namespace ToDo.Application.Services
{
  public interface ITodoItemService
  {
    Task<TodoItemDto?> GetByIdAsync(int id);
    Task<IEnumerable<TodoItemDto>> GetAllAsync();
    Task<IEnumerable<TodoItemDto>> GetByUserIdAsync(string userId);
    Task<IEnumerable<TodoItemDto>> GetByListIdAsync(int listId);
    Task<TodoItemDto> CreateAsync(CreateTodoItemDto todoItemDto);
    Task<TodoItemDto> UpdateAsync(int id, UpdateTodoItemDto todoItemDto);
    Task DeleteAsync(int id);
    Task<PaginatedResultDto<TodoItemDto>> GetPagedAsync(string userId, PaginationRequestDto paginationRequestDto);
  }
}