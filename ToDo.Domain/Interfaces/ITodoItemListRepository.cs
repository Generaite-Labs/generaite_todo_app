using ToDo.Domain.Entities;
using ToDo.Domain.Common;

namespace ToDo.Domain.Interfaces
{
  public interface ITodoItemListRepository
  {
    Task<TodoItemList?> GetByIdAsync(int id);
    Task<IEnumerable<TodoItemList>> GetAllAsync();
    Task<IEnumerable<TodoItemList>> GetByUserIdAsync(string userId);
    Task<TodoItemList> AddAsync(TodoItemList todoItemList);
    Task UpdateAsync(TodoItemList todoItemList);
    Task DeleteAsync(int id);
    Task<PaginatedResult<TodoItemList>> GetPagedAsync(string userId, PaginationRequest paginationRequest);
  }
}