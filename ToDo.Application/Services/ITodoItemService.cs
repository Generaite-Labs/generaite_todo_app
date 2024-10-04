using ToDo.Application.DTOs;

public interface ITodoItemService
{
    Task<TodoItemDto?> GetByIdAsync(string userId, int id);
    Task<IEnumerable<TodoItemDto>> GetAllAsync(string userId);
    Task<TodoItemDto> CreateAsync(string userId, CreateTodoItemDto createDto);
    Task<TodoItemDto> UpdateAsync(string userId, int id, UpdateTodoItemDto updateDto);
    Task DeleteAsync(string userId, int id);
    Task<PaginatedResultDto<TodoItemDto>> GetPagedAsync(string userId, PaginationRequestDto paginationRequestDto);
}