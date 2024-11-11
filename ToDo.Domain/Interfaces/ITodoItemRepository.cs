using ToDo.Domain.Entities;
using ToDo.Domain.Common;

namespace ToDo.Domain.Interfaces
{
    public interface ITodoItemRepository : IBaseRepository<TodoItem, Guid>
    {
        Task<IEnumerable<TodoItem>> GetByUserIdAsync(string userId);
        Task<PaginatedResult<TodoItem>> GetPagedAsync(string userId, PaginationRequest paginationRequest);
    }
}