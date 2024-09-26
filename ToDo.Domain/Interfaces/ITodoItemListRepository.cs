using ToDo.Domain.Entities;

namespace ToDo.Domain.Interfaces
{
  public interface ITodoItemListRepository
  {
    Task<TodoItemList?> GetByIdAsync(int id);
    Task<IEnumerable<TodoItemList>> GetAllAsync();
    Task<IEnumerable<TodoItemList>> GetByUserIdAsync(string userId);  // Changed parameter type to string
    Task<TodoItemList> AddAsync(TodoItemList todoItemList);
    Task UpdateAsync(TodoItemList todoItemList);
    Task DeleteAsync(int id);
  }
}