using ToDo.Application.DTOs;

namespace ToDo.Application.Services
{
    public interface ITodoItemListService
    {
        Task<TodoItemListDto?> GetByIdAsync(int id);
        Task<IEnumerable<TodoItemListDto>> GetAllAsync();
        Task<IEnumerable<TodoItemListDto>> GetByUserIdAsync(string userId);
        Task<TodoItemListDto> CreateAsync(CreateTodoItemListDto createDto);
        Task<TodoItemListDto> UpdateAsync(UpdateTodoItemListDto updateDto);
        Task DeleteAsync(int id);
    }
}