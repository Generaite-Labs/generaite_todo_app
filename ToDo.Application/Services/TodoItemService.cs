using AutoMapper;
using ToDo.Domain.Entities;
using ToDo.Domain.Interfaces;
using ToDo.Domain.Common;
using ToDo.Application.DTOs;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ToDo.Application.Services
{
  public class TodoItemService : ITodoItemService
  {
    private readonly ITodoItemRepository _todoItemRepository;
    private readonly ILogger<TodoItemService> _logger;
    private readonly IMapper _mapper;

    public TodoItemService(ITodoItemRepository todoItemRepository, ILogger<TodoItemService> logger, IMapper mapper)
    {
      _todoItemRepository = todoItemRepository;
      _logger = logger;
      _mapper = mapper;
    }

    public async Task<TodoItemDto?> GetByIdAsync(int id)
    {
      _logger.LogInformation("Getting TodoItem by ID: {TodoItemId}", id);
      var todoItem = await _todoItemRepository.GetByIdAsync(id);
      if (todoItem == null)
      {
        _logger.LogWarning("TodoItem not found: {TodoItemId}", id);
        return null;
      }
      return _mapper.Map<TodoItemDto>(todoItem);
    }

    public async Task<IEnumerable<TodoItemDto>> GetAllAsync()
    {
      _logger.LogInformation("Getting all TodoItems");
      var todoItems = await _todoItemRepository.GetAllAsync();
      return _mapper.Map<IEnumerable<TodoItemDto>>(todoItems) ?? Enumerable.Empty<TodoItemDto>();
    }

    public async Task<IEnumerable<TodoItemDto>> GetByUserIdAsync(string userId)
    {
      _logger.LogInformation("Getting TodoItems for user: {UserId}", userId);
      var todoItems = await _todoItemRepository.GetByUserIdAsync(userId);
      return _mapper.Map<IEnumerable<TodoItemDto>>(todoItems) ?? Enumerable.Empty<TodoItemDto>();
    }

    public async Task<TodoItemDto> CreateAsync(CreateTodoItemDto createDto)
    {
      _logger.LogInformation("Creating new TodoItem: {@CreateTodoItemDto}", createDto);
      var todoItem = _mapper.Map<TodoItem>(createDto);
      if (todoItem == null)
      {
        throw new InvalidOperationException("Failed to map CreateTodoItemDto to TodoItem");
      }
      todoItem.CreatedAt = DateTime.UtcNow;
      todoItem.UpdatedAt = DateTime.UtcNow;

      var createdItem = await _todoItemRepository.AddAsync(todoItem);
      _logger.LogInformation("Created TodoItem with ID: {TodoItemId}", createdItem.Id);
      return _mapper.Map<TodoItemDto>(createdItem) ?? throw new InvalidOperationException("Failed to map created TodoItem to TodoItemDto");
    }

    public async Task<TodoItemDto> UpdateAsync(int id, UpdateTodoItemDto updateDto)
    {
      _logger.LogInformation("Updating TodoItem: {TodoItemId}, {@UpdateTodoItemDto}", id, updateDto);
      var existingItem = await _todoItemRepository.GetByIdAsync(id);
      if (existingItem == null)
      {
        _logger.LogWarning("Failed to update TodoItem. Item not found: {TodoItemId}", id);
        throw new KeyNotFoundException($"TodoItem with id {id} not found.");
      }

      _mapper.Map(updateDto, existingItem);
      existingItem.UpdatedAt = DateTime.UtcNow;

      await _todoItemRepository.UpdateAsync(existingItem);
      _logger.LogInformation("Updated TodoItem: {TodoItemId}", id);
      return _mapper.Map<TodoItemDto>(existingItem) ?? throw new InvalidOperationException("Failed to map updated TodoItem to TodoItemDto");
    }

    public async Task DeleteAsync(int id)
    {
      _logger.LogInformation("Deleting TodoItem: {TodoItemId}", id);
      await _todoItemRepository.DeleteAsync(id);
      _logger.LogInformation("Deleted TodoItem: {TodoItemId}", id);
    }

    public async Task<PaginatedResultDto<TodoItemDto>> GetPagedAsync(string userId, PaginationRequestDto paginationRequestDto)
    {
      _logger.LogInformation("Getting paged TodoItems for user: {UserId}, {@PaginationRequestDto}", userId, paginationRequestDto);
      var stopwatch = Stopwatch.StartNew();

      var domainPaginationRequest = new PaginationRequest(paginationRequestDto.Limit, paginationRequestDto.Cursor);
      var result = await _todoItemRepository.GetPagedAsync(userId, domainPaginationRequest);

      stopwatch.Stop();
      _logger.LogInformation("Retrieved paged TodoItems for user: {UserId}. Took {ElapsedMilliseconds}ms", userId, stopwatch.ElapsedMilliseconds);

      return new PaginatedResultDto<TodoItemDto>
      {
        Items = _mapper.Map<List<TodoItemDto>>(result.Items) ?? new List<TodoItemDto>(),
        NextCursor = result.NextCursor
      };
    }
  }
}