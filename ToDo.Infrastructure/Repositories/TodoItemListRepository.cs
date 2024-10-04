using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Entities;
using ToDo.Domain.Interfaces;
using ToDo.Domain.Common;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ToDo.Infrastructure.Repositories
{
  public class TodoItemListRepository : BaseRepository<TodoItemList>, ITodoItemListRepository
  {
    private readonly ILogger<TodoItemListRepository> _logger;

    public TodoItemListRepository(TodoDbContext context, ILogger<TodoItemListRepository> logger) : base(context)
    {
      _logger = logger;
    }

    public async Task<TodoItemList?> GetByIdAsync(int id)
    {
      _logger.LogInformation("Getting TodoItemList by ID: {TodoItemListId}", id);
      var todoItemList = await _context.Set<TodoItemList>().FindAsync(id);
      if (todoItemList == null)
      {
        _logger.LogWarning("TodoItemList not found: {TodoItemListId}", id);
      }
      return todoItemList;
    }

    public async Task<IEnumerable<TodoItemList>> GetAllAsync()
    {
      _logger.LogInformation("Getting all TodoItemLists");
      var stopwatch = Stopwatch.StartNew();
      var todoItemLists = await _context.Set<TodoItemList>().ToListAsync();
      stopwatch.Stop();
      _logger.LogInformation("Retrieved {Count} TodoItemLists. Took {ElapsedMilliseconds}ms", todoItemLists.Count, stopwatch.ElapsedMilliseconds);
      return todoItemLists;
    }

    public async Task<IEnumerable<TodoItemList>> GetByUserIdAsync(string userId)
    {
      _logger.LogInformation("Getting TodoItemLists for user: {UserId}", userId);
      var stopwatch = Stopwatch.StartNew();
      var todoItemLists = await _context.Set<TodoItemList>().Where(t => t.UserId == userId).ToListAsync();
      stopwatch.Stop();
      _logger.LogInformation("Retrieved {Count} TodoItemLists for user {UserId}. Took {ElapsedMilliseconds}ms", todoItemLists.Count, userId, stopwatch.ElapsedMilliseconds);
      return todoItemLists;
    }

    public async Task<TodoItemList> AddAsync(TodoItemList todoItemList)
    {
      _logger.LogInformation("Adding new TodoItemList: {@TodoItemList}", todoItemList);
      await _context.Set<TodoItemList>().AddAsync(todoItemList);
      await _context.SaveChangesAsync();
      _logger.LogInformation("Added new TodoItemList with ID: {TodoItemListId}", todoItemList.Id);
      return todoItemList;
    }

    public async Task UpdateAsync(TodoItemList todoItemList)
    {
      _logger.LogInformation("Updating TodoItemList: {@TodoItemList}", todoItemList);
      _context.Set<TodoItemList>().Update(todoItemList);
      await _context.SaveChangesAsync();
      _logger.LogInformation("Updated TodoItemList with ID: {TodoItemListId}", todoItemList.Id);
    }

    public async Task DeleteAsync(int id)
    {
      _logger.LogInformation("Deleting TodoItemList with ID: {TodoItemListId}", id);
      var todoItemList = await _context.Set<TodoItemList>().FindAsync(id);
      if (todoItemList != null)
      {
        _context.Set<TodoItemList>().Remove(todoItemList);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Deleted TodoItemList with ID: {TodoItemListId}", id);
      }
      else
      {
        _logger.LogWarning("Attempted to delete non-existent TodoItemList with ID: {TodoItemListId}", id);
      }
    }

    public async Task<PaginatedResult<TodoItemList>> GetPagedAsync(string userId, PaginationRequest paginationRequest)
    {
      _logger.LogInformation("Getting paged TodoItemLists for user: {UserId}, {@PaginationRequest}", userId, paginationRequest);
      var stopwatch = Stopwatch.StartNew();
      var query = _context.Set<TodoItemList>().Where(t => t.UserId == userId);
      var result = await GetPagedAsync(query, t => t.Id, paginationRequest);
      stopwatch.Stop();
      _logger.LogInformation("Retrieved paged TodoItemLists for user: {UserId}. Took {ElapsedMilliseconds}ms", userId, stopwatch.ElapsedMilliseconds);
      return result;
    }
  }
}