using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Entities;
using ToDo.Domain.Interfaces;
using ToDo.Domain.Common;

namespace ToDo.Infrastructure.Repositories
{
  public class TodoItemListRepository : BaseRepository<TodoItemList>, ITodoItemListRepository
  {
    public TodoItemListRepository(TodoDbContext context) : base(context)
    {
    }

    public async Task<TodoItemList?> GetByIdAsync(int id)
    {
      return await _context.Set<TodoItemList>().FindAsync(id);
    }

    public async Task<IEnumerable<TodoItemList>> GetAllAsync()
    {
      return await _context.Set<TodoItemList>().ToListAsync();
    }

    public async Task<IEnumerable<TodoItemList>> GetByUserIdAsync(string userId)
    {
      return await _context.Set<TodoItemList>().Where(t => t.UserId == userId).ToListAsync();
    }

    public async Task<TodoItemList> AddAsync(TodoItemList todoItemList)
    {
      await _context.Set<TodoItemList>().AddAsync(todoItemList);
      await _context.SaveChangesAsync();
      return todoItemList;
    }

    public async Task UpdateAsync(TodoItemList todoItemList)
    {
      _context.Set<TodoItemList>().Update(todoItemList);
      await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
      var todoItemList = await _context.Set<TodoItemList>().FindAsync(id);
      if (todoItemList != null)
      {
        _context.Set<TodoItemList>().Remove(todoItemList);
        await _context.SaveChangesAsync();
      }
    }

    public async Task<PaginatedResult<TodoItemList>> GetPagedAsync(string userId, PaginationRequest paginationRequest)
    {
      var query = _context.Set<TodoItemList>().Where(t => t.UserId == userId);
      return await GetPagedAsync(query, t => t.Id, paginationRequest);
    }
  }
}