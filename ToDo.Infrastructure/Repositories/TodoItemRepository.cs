using Microsoft.EntityFrameworkCore;
using ToDo.Domain.Entities;
using ToDo.Domain.Interfaces;
using ToDo.Domain.Common;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ToDo.Infrastructure.Repositories
{
  public class TodoItemRepository : BaseRepository<TodoItem>, ITodoItemRepository
  {
    private readonly ILogger<TodoItemRepository> _logger;

    public TodoItemRepository(ApplicationDbContext context, ILogger<TodoItemRepository> logger) : base(context)
    {
      _logger = logger;
    }

    public async Task<TodoItem?> GetByIdAsync(int id)
    {
      _logger.LogInformation("Getting TodoItem by ID: {TodoItemId}", id);
      var todoItem = await _context.Set<TodoItem>().FindAsync(id);
      if (todoItem == null)
      {
        _logger.LogWarning("TodoItem not found: {TodoItemId}", id);
      }
      return todoItem;
    }

    public async Task<IEnumerable<TodoItem>> GetAllAsync()
    {
      _logger.LogInformation("Getting all TodoItems");
      var stopwatch = Stopwatch.StartNew();
      var result = await _context.Set<TodoItem>().ToListAsync();
      stopwatch.Stop();
      _logger.LogInformation("Retrieved {Count} TodoItems. Took {ElapsedMilliseconds}ms", result.Count, stopwatch.ElapsedMilliseconds);
      return result;
    }

    public async Task<IEnumerable<TodoItem>> GetByUserIdAsync(string userId)
    {
      _logger.LogInformation("Getting TodoItems for user: {UserId}", userId);
      var stopwatch = Stopwatch.StartNew();
      var result = await _context.Set<TodoItem>().Where(t => t.UserId == userId).ToListAsync();
      stopwatch.Stop();
      _logger.LogInformation("Retrieved {Count} TodoItems for user {UserId}. Took {ElapsedMilliseconds}ms", result.Count, userId, stopwatch.ElapsedMilliseconds);
      return result;
    }

    public async Task<TodoItem> AddAsync(TodoItem todoItem)
    {
      _logger.LogInformation("Adding new TodoItem: {@TodoItem}", todoItem);
      await _context.Set<TodoItem>().AddAsync(todoItem);
      await _context.SaveChangesAsync();
      _logger.LogInformation("Added new TodoItem with ID: {TodoItemId}", todoItem.Id);
      return todoItem;
    }

    public async Task UpdateAsync(TodoItem todoItem)
    {
      _logger.LogInformation("Updating TodoItem: {@TodoItem}", todoItem);
      _context.Set<TodoItem>().Update(todoItem);
      await _context.SaveChangesAsync();
      _logger.LogInformation("Updated TodoItem with ID: {TodoItemId}", todoItem.Id);
    }

    public async Task DeleteAsync(int id)
    {
      _logger.LogInformation("Deleting TodoItem with ID: {TodoItemId}", id);
      var todoItem = await _context.Set<TodoItem>().FindAsync(id);
      if (todoItem != null)
      {
        _context.Set<TodoItem>().Remove(todoItem);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Deleted TodoItem with ID: {TodoItemId}", id);
      }
      else
      {
        _logger.LogWarning("Attempted to delete non-existent TodoItem with ID: {TodoItemId}", id);
      }
    }

    public async Task<PaginatedResult<TodoItem>> GetPagedAsync(string userId, PaginationRequest paginationRequest)
    {
      _logger.LogInformation("Getting paged TodoItems for user: {UserId}, {@PaginationRequest}", userId, paginationRequest);
      var stopwatch = Stopwatch.StartNew();
      var query = _context.Set<TodoItem>().Where(t => t.UserId == userId);
      var result = await GetPagedAsync(query, t => t.Id, paginationRequest);
      stopwatch.Stop();
      _logger.LogInformation("Retrieved paged TodoItems for user: {UserId}. Took {ElapsedMilliseconds}ms", 
        userId, stopwatch.ElapsedMilliseconds);
      return result;
    }
  }
}