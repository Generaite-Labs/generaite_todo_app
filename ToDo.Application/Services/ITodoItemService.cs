using ToDo.Application.DTOs;

namespace ToDo.Application.Interfaces
{
    public interface ITodoItemService
    {
        Task<TodoItemDto?> GetByIdAsync(string userId, int id);
        Task<IEnumerable<TodoItemDto>> GetAllAsync(string userId);
        Task<TodoItemDto> CreateAsync(string userId, CreateTodoItemDto createDto);
        Task<TodoItemDto> UpdateAsync(string userId, int id, UpdateTodoItemDto updateDto);
        Task StartTodoItemAsync(string userId, int id);
        Task StopTodoItemAsync(string userId, int id);
        Task CompleteTodoItemAsync(string userId, int id);
        Task AssignTodoItemAsync(string userId, int id, string assignedUserId);
        Task DeleteAsync(string userId, int id);
        Task<PaginatedResultDto<TodoItemDto>> GetPagedAsync(string userId, PaginationRequestDto paginationRequestDto);
    }
}