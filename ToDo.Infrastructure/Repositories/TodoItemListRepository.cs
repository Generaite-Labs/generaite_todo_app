using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Entities;
using ToDo.Domain.Interfaces;

namespace ToDo.Infrastructure.Repositories
{
  public class TodoItemListRepository : ITodoItemListRepository
  {
    private readonly TodoDbContext _context;

    public TodoItemListRepository(TodoDbContext context)
    {
      _context = context;
    }

    public async Task<TodoItemList?> GetByIdAsync(int id)
    {
      return await _context.TodoItemLists.FindAsync(id);
    }

    public async Task<IEnumerable<TodoItemList>> GetAllAsync()
    {
      return await _context.TodoItemLists.ToListAsync();
    }

    public async Task<IEnumerable<TodoItemList>> GetByUserIdAsync(string userId)  // Changed parameter type to string
    {
      return await _context.TodoItemLists.Where(t => t.UserId == userId).ToListAsync();
    }

    public async Task<TodoItemList> AddAsync(TodoItemList todoItemList)
    {
      await _context.TodoItemLists.AddAsync(todoItemList);
      await _context.SaveChangesAsync();
      return todoItemList;
    }

    public async Task UpdateAsync(TodoItemList todoItemList)
    {
      _context.TodoItemLists.Update(todoItemList);
      await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
      var todoItemList = await _context.TodoItemLists.FindAsync(id);
      if (todoItemList != null)
      {
        _context.TodoItemLists.Remove(todoItemList);
        await _context.SaveChangesAsync();
      }
    }
  }
}