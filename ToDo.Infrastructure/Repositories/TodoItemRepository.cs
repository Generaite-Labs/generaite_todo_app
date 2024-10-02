using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Entities;
using ToDo.Domain.Interfaces;
using ToDo.Domain.Common;

namespace ToDo.Infrastructure.Repositories
{
  public class TodoItemRepository : BaseRepository<TodoItem>, ITodoItemRepository
  {
    public TodoItemRepository(TodoDbContext context) : base(context)
    {
    }

    public async Task<TodoItem?> GetByIdAsync(int id)
    {
      return await _context.Set<TodoItem>().FindAsync(id);
    }

    public async Task<IEnumerable<TodoItem>> GetAllAsync()
    {
      return await _context.Set<TodoItem>().ToListAsync();
    }

    public async Task<IEnumerable<TodoItem>> GetByUserIdAsync(string userId)
    {
      return await _context.Set<TodoItem>().Where(t => t.UserId == userId).ToListAsync();
    }

    public async Task<IEnumerable<TodoItem>> GetByListIdAsync(int listId)
    {
      return await _context.Set<TodoItem>().Where(t => t.TodoItemListId == listId).ToListAsync();
    }

    public async Task<TodoItem> AddAsync(TodoItem todoItem)
    {
      await _context.Set<TodoItem>().AddAsync(todoItem);
      await _context.SaveChangesAsync();
      return todoItem;
    }

    public async Task UpdateAsync(TodoItem todoItem)
    {
      _context.Set<TodoItem>().Update(todoItem);
      await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
      var todoItem = await _context.Set<TodoItem>().FindAsync(id);
      if (todoItem != null)
      {
        _context.Set<TodoItem>().Remove(todoItem);
        await _context.SaveChangesAsync();
      }
    }

    public async Task<PaginatedResult<TodoItem>> GetPagedAsync(string userId, PaginationRequest paginationRequest)
    {
      var query = _context.Set<TodoItem>().Where(t => t.UserId == userId);
      return await GetPagedAsync(query, t => t.Id, paginationRequest);
    }
  }
}
