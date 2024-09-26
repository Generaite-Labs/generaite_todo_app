using System.Collections.Generic;
using System.Threading.Tasks;
using ToDo.Domain.Entities;

namespace ToDo.Domain.Interfaces
{
  public interface ITodoItemRepository
  {
    Task<TodoItem?> GetByIdAsync(int id);
    Task<IEnumerable<TodoItem>> GetAllAsync();
    Task<IEnumerable<TodoItem>> GetByUserIdAsync(string userId);
    Task<IEnumerable<TodoItem>> GetByListIdAsync(int listId);
    Task<TodoItem> AddAsync(TodoItem todoItem);
    Task UpdateAsync(TodoItem todoItem);
    Task DeleteAsync(int id);
  }
}