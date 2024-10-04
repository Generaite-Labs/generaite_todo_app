using ToDo.Domain.Entities;
using ToDo.Domain.Interfaces;
using ToDo.Domain.Common;
using ToDo.Application.DTOs;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ToDo.Application.Services
{
  public class TodoItemListService : ITodoItemListService
  {
    private readonly ITodoItemListRepository _todoItemListRepository;
    private readonly ILogger<TodoItemListService> _logger;

    public TodoItemListService(ITodoItemListRepository todoItemListRepository, ILogger<TodoItemListService> logger)
    {
      _todoItemListRepository = todoItemListRepository;
      _logger = logger;
    }

    public async Task<TodoItemListDto?> GetByIdAsync(int id)
    {
      _logger.LogInformation("Getting TodoItemList by ID: {TodoItemListId}", id);
      var todoItemList = await _todoItemListRepository.GetByIdAsync(id);
      if (todoItemList == null)
      {
        _logger.LogWarning("TodoItemList not found: {TodoItemListId}", id);
      }
      return todoItemList != null ? MapToDto(todoItemList) : null;
    }

    public async Task<IEnumerable<TodoItemListDto>> GetAllAsync()
    {
      _logger.LogInformation("Getting all TodoItemLists");
      var stopwatch = Stopwatch.StartNew();
      var todoItemLists = await _todoItemListRepository.GetAllAsync();
      stopwatch.Stop();
      _logger.LogInformation("Retrieved {Count} TodoItemLists. Took {ElapsedMilliseconds}ms", todoItemLists.Count(), stopwatch.ElapsedMilliseconds);
      return todoItemLists.Select(MapToDto);
    }

    public async Task<IEnumerable<TodoItemListDto>> GetByUserIdAsync(string userId)
    {
      _logger.LogInformation("Getting TodoItemLists for user: {UserId}", userId);
      var stopwatch = Stopwatch.StartNew();
      var todoItemLists = await _todoItemListRepository.GetByUserIdAsync(userId);
      stopwatch.Stop();
      _logger.LogInformation("Retrieved {Count} TodoItemLists for user {UserId}. Took {ElapsedMilliseconds}ms", todoItemLists.Count(), userId, stopwatch.ElapsedMilliseconds);
      return todoItemLists.Select(MapToDto);
    }

    public async Task<TodoItemListDto> CreateAsync(CreateTodoItemListDto createDto)
    {
      _logger.LogInformation("Creating new TodoItemList: {@CreateTodoItemListDto}", createDto);
      var todoItemList = new TodoItemList
      {
        Name = createDto.Name,
        UserId = createDto.UserId,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
      };

      var createdList = await _todoItemListRepository.AddAsync(todoItemList);
      _logger.LogInformation("Created new TodoItemList with ID: {TodoItemListId}", createdList.Id);
      return MapToDto(createdList);
    }

    public async Task<TodoItemListDto> UpdateAsync(UpdateTodoItemListDto updateDto)
    {
      _logger.LogInformation("Updating TodoItemList: {@UpdateTodoItemListDto}", updateDto);
      var existingList = await _todoItemListRepository.GetByIdAsync(updateDto.Id);
      if (existingList == null)
      {
        _logger.LogWarning("Attempted to update non-existent TodoItemList with ID: {TodoItemListId}", updateDto.Id);
        throw new KeyNotFoundException($"TodoItemList with id {updateDto.Id} not found.");
      }

      existingList.Name = updateDto.Name;
      existingList.UpdatedAt = DateTime.UtcNow;

      await _todoItemListRepository.UpdateAsync(existingList);
      _logger.LogInformation("Updated TodoItemList with ID: {TodoItemListId}", existingList.Id);
      return MapToDto(existingList);
    }

    public async Task DeleteAsync(int id)
    {
      _logger.LogInformation("Deleting TodoItemList with ID: {TodoItemListId}", id);
      await _todoItemListRepository.DeleteAsync(id);
      _logger.LogInformation("Deleted TodoItemList with ID: {TodoItemListId}", id);
    }

    public async Task<PaginatedResultDto<TodoItemListDto>> GetPagedAsync(string userId, PaginationRequestDto paginationRequestDto)
    {
      _logger.LogInformation("Getting paged TodoItemLists for user: {UserId}, {@PaginationRequestDto}", userId, paginationRequestDto);
      var stopwatch = Stopwatch.StartNew();
      var domainPaginationRequest = new PaginationRequest(paginationRequestDto.Limit, paginationRequestDto.Cursor);
      var result = await _todoItemListRepository.GetPagedAsync(userId, domainPaginationRequest);
      stopwatch.Stop();
      _logger.LogInformation("Retrieved paged TodoItemLists for user: {UserId}. Took {ElapsedMilliseconds}ms", userId, stopwatch.ElapsedMilliseconds);

      return new PaginatedResultDto<TodoItemListDto>
      {
        Items = result.Items.Select(MapToDto).ToList(),
        NextCursor = result.NextCursor
      };
    }

    private static TodoItemListDto MapToDto(TodoItemList todoItemList)
    {
      return new TodoItemListDto
      {
        Id = todoItemList.Id,
        Name = todoItemList.Name,
        UserId = todoItemList.UserId,
        CreatedAt = todoItemList.CreatedAt,
        UpdatedAt = todoItemList.UpdatedAt
      };
    }
  }
}