using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Entities;
using ToDo.Domain.Interfaces;

namespace ToDo.Infrastructure.Repositories
{
  public class TodoItemRepository : ITodoItemRepository
  {
    private readonly TodoDbContext _context;

    public TodoItemRepository(TodoDbContext context)
    {
      _context = context;
    }

    public async Task<TodoItem?> GetByIdAsync(int id)
    {
      return await _context.TodoItems.FindAsync(id);
    }

    public async Task<IEnumerable<TodoItem>> GetAllAsync()
    {
      return await _context.TodoItems.ToListAsync();
    }

    public async Task<IEnumerable<TodoItem>> GetByUserIdAsync(string userId)
    {
      return await _context.TodoItems.Where(t => t.UserId == userId).ToListAsync();
    }

    public async Task<IEnumerable<TodoItem>> GetByListIdAsync(int listId)
    {
      return await _context.TodoItems.Where(t => t.TodoItemListId == listId).ToListAsync();
    }

    public async Task<TodoItem> AddAsync(TodoItem todoItem)
    {
      await _context.TodoItems.AddAsync(todoItem);
      await _context.SaveChangesAsync();
      return todoItem;
    }

    public async Task UpdateAsync(TodoItem todoItem)
    {
      _context.TodoItems.Update(todoItem);
      await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
      var todoItem = await _context.TodoItems.FindAsync(id);
      if (todoItem != null)
      {
        _context.TodoItems.Remove(todoItem);
        await _context.SaveChangesAsync();
      }
    }
  }
}