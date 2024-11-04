using AutoMapper;
using ToDo.Domain.Entities;
using ToDo.Domain.Interfaces;
using ToDo.Domain.Common;
using ToDo.Application.DTOs;
using ToDo.Application.Exceptions;
using ToDo.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;

namespace ToDo.Application.Services
{
  public class TodoItemService : ITodoItemService
  {
    private readonly ITodoItemRepository _repository;
    private readonly ILogger<TodoItemService> _logger;
    private readonly IMapper _mapper;
    private readonly IDomainEventService _domainEventService;

    public TodoItemService(ITodoItemRepository repository, ILogger<TodoItemService> logger, IMapper mapper, IDomainEventService domainEventService)
    {
      _repository = repository;
      _logger = logger;
      _mapper = mapper;
      _domainEventService = domainEventService;
    }

    public async Task<TodoItemDto?> GetByIdAsync(string userId, int id)
    {
      _logger.LogInformation("Getting TodoItem by ID: {TodoItemId} for user: {UserId}", id, userId);
      var todoItem = await _repository.GetByIdAsync(id);
      if (todoItem == null)
      {
        _logger.LogWarning("TodoItem not found: {TodoItemId}, {UserId}", id, userId);
        return null;
      }
      if (todoItem.UserId != userId)
      {
        _logger.LogWarning("Unauthorized access to TodoItem: {TodoItemId}, {UserId}", id, userId);
        throw new UnauthorizedTodoItemAccessException(userId, id);
      }
      return _mapper.Map<TodoItemDto>(todoItem);
    }

    public async Task<IEnumerable<TodoItemDto>> GetAllAsync(string userId)
    {
      _logger.LogInformation("Getting all TodoItems for user: {UserId}", userId);
      var todoItems = await _repository.GetByUserIdAsync(userId);
      
      if (todoItems == null || !todoItems.Any())
      {
        _logger.LogInformation("No TodoItems found for user: {UserId}", userId);
        return Enumerable.Empty<TodoItemDto>();
      }
      
      var mappedItems = _mapper.Map<IEnumerable<TodoItemDto>>(todoItems);
      
      if (mappedItems == null || !mappedItems.Any())
      {
        _logger.LogWarning("Failed to map TodoItems to DTOs for user: {UserId}", userId);
        throw new InvalidTodoItemMappingException($"Failed to map TodoItems to DTOs for user: {userId}");
      }
      
      return mappedItems;
    }

    public async Task<TodoItemDto> CreateAsync(string userId, CreateTodoItemDto createDto)
    {
      try
      {
        var todoItem = TodoItem.CreateTodoItem(
            createDto.Title,
            createDto.Description,
            userId,
            createDto.DueDate?.ToUniversalTime() // Ensure DueDate is in UTC
        );

        var createdTodoItem = await _repository.AddAsync(todoItem);

        var todoItemDto = _mapper.Map<TodoItemDto>(createdTodoItem);
        if (todoItemDto == null)
        {
            throw new InvalidTodoItemMappingException("Failed to map created TodoItem to TodoItemDto");
        }

        return todoItemDto;
      }
      catch (Exception ex) when (!(ex is InvalidTodoItemMappingException))
      {
        throw new TodoItemOperationException("Create", ex.Message);
      }
    }

    public async Task<TodoItemDto> UpdateAsync(string userId, int id, UpdateTodoItemDto updateDto)
    {
      _logger.LogInformation("Updating TodoItem: {TodoItemId} for user: {UserId}, {@UpdateTodoItemDto}", id, userId, updateDto);
      var existingItem = await _repository.GetByIdAsync(id);
      if (existingItem == null || existingItem.UserId != userId)
      {
        _logger.LogWarning("Failed to update TodoItem. Item not found or unauthorized access: {TodoItemId}, {UserId}", id, userId);
        throw new UnauthorizedTodoItemAccessException(userId, id);
      }

      existingItem.UpdateTodoItem(
          updateDto.Title,
          updateDto.Description,
          updateDto.DueDate?.ToUniversalTime() // Ensure DueDate is in UTC
      );

      try
      {
        await _repository.UpdateAsync(existingItem);
        await _repository.UpdateAsync(existingItem);
        _logger.LogInformation("Updated TodoItem: {TodoItemId} for user: {UserId}", id, userId);
        return _mapper.Map<TodoItemDto>(existingItem) ?? 
               throw new InvalidTodoItemMappingException("Failed to map updated TodoItem to TodoItemDto");
      }
      catch (Exception ex)
      {
        throw new TodoItemOperationException("Update", $"Failed to update TodoItem with ID {id}", ex);
      }
    }

    public async Task StartTodoItemAsync(string userId, int id)
    {
      var todoItem = await GetAndValidateTodoItemAsync(userId, id);
      todoItem.StartTodoItem();
      await _repository.UpdateAsync(todoItem);
    }

    public async Task StopTodoItemAsync(string userId, int id)
    {
      var todoItem = await GetAndValidateTodoItemAsync(userId, id);
      todoItem.StopTodoItem();
      await _repository.UpdateAsync(todoItem);
    }

    public async Task CompleteTodoItemAsync(string userId, int id)
    {
      var todoItem = await GetAndValidateTodoItemAsync(userId, id);
      todoItem.CompleteTodoItem();
      await _repository.UpdateAsync(todoItem);
    }

    public async Task AssignTodoItemAsync(string userId, int id, string assignedUserId)
    {
      var todoItem = await GetAndValidateTodoItemAsync(userId, id);
      todoItem.AssignTodoItem(assignedUserId);
      await _repository.UpdateAsync(todoItem);
    }

    public async Task DeleteAsync(string userId, int id)
    {
      _logger.LogInformation("Deleting TodoItem: {TodoItemId} for user: {UserId}", id, userId);
      var existingItem = await _repository.GetByIdAsync(id);
      if (existingItem == null || existingItem.UserId != userId)
      {
        _logger.LogWarning("Failed to delete TodoItem. Item not found or unauthorized access: {TodoItemId}, {UserId}", id, userId);
        throw new UnauthorizedTodoItemAccessException(userId, id);
      }

      try
      {
        await _repository.DeleteAsync(id);
        _logger.LogInformation("Deleted TodoItem: {TodoItemId} for user: {UserId}", id, userId);
      }
      catch (Exception ex)
      {
        throw new TodoItemOperationException("Delete", $"Failed to delete TodoItem with ID {id}", ex);
      }
    }

    public async Task<PaginatedResultDto<TodoItemDto>> GetPagedAsync(string userId, PaginationRequestDto paginationRequestDto)
    {
      _logger.LogInformation("Getting paged TodoItems for user: {UserId}, {@PaginationRequestDto}", userId, paginationRequestDto);
      var stopwatch = Stopwatch.StartNew();

      var domainPaginationRequest = new PaginationRequest(paginationRequestDto.Limit, paginationRequestDto.Cursor);
      var result = await _repository.GetPagedAsync(userId, domainPaginationRequest);

      stopwatch.Stop();
      _logger.LogInformation("Retrieved paged TodoItems for user: {UserId}. Took {ElapsedMilliseconds}ms", userId, stopwatch.ElapsedMilliseconds);

      var mappedItems = _mapper.Map<List<TodoItemDto>>(result.Items);
      if (mappedItems == null || !mappedItems.Any())
      {
        throw new InvalidTodoItemMappingException("Failed to map paged TodoItems to DTOs or the result is empty");
      }

      return new PaginatedResultDto<TodoItemDto>
      {
        Items = mappedItems,
        NextCursor = result.NextCursor
      };
    }

    private async Task<TodoItem> GetAndValidateTodoItemAsync(string userId, int id)
    {
      var todoItem = await _repository.GetByIdAsync(id);
      if (todoItem == null || todoItem.UserId != userId)
      {
        _logger.LogWarning("TodoItem not found or unauthorized access: {TodoItemId}, {UserId}", id, userId);
        throw new UnauthorizedTodoItemAccessException(userId, id);
      }
      return todoItem;
    }
  }
}
