using ToDo.Domain.Entities;
using ToDo.Domain.Common;

namespace ToDo.Domain.Interfaces
{
    public interface ITodoItemRepository
    {
        Task<TodoItem?> GetByIdAsync(int id);
        Task<IEnumerable<TodoItem>> GetAllAsync();
        Task<IEnumerable<TodoItem>> GetByUserIdAsync(string userId);
        Task<TodoItem> AddAsync(TodoItem todoItem);
        Task UpdateAsync(TodoItem todoItem);
        Task DeleteAsync(int id);
        Task<PaginatedResult<TodoItem>> GetPagedAsync(string userId, PaginationRequest paginationRequest);
    }
}