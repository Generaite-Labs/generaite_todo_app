using ToDo.Domain.Entities;
using ToDo.Domain.Interfaces;
using ToDo.Application.DTOs;

namespace ToDo.Application.Services
{
    public class TodoItemService : ITodoItemService
    {
        private readonly ITodoItemRepository _todoItemRepository;

        public TodoItemService(ITodoItemRepository todoItemRepository)
        {
            _todoItemRepository = todoItemRepository;
        }

        public async Task<TodoItemDto?> GetByIdAsync(int id)
        {
            var todoItem = await _todoItemRepository.GetByIdAsync(id);
            return todoItem != null ? MapToDto(todoItem) : null;
        }

        public async Task<IEnumerable<TodoItemDto>> GetAllAsync()
        {
            var todoItems = await _todoItemRepository.GetAllAsync();
            return todoItems.Select(MapToDto);
        }

        public async Task<IEnumerable<TodoItemDto>> GetByUserIdAsync(string userId)
        {
            var todoItems = await _todoItemRepository.GetByUserIdAsync(userId);
            return todoItems.Select(MapToDto);
        }

        public async Task<IEnumerable<TodoItemDto>> GetByListIdAsync(int listId)
        {
            var todoItems = await _todoItemRepository.GetByListIdAsync(listId);
            return todoItems.Select(MapToDto);
        }

        public async Task<TodoItemDto> CreateAsync(CreateTodoItemDto createDto)
        {
            var todoItem = new TodoItem
            {
                Title = createDto.Title,
                Description = createDto.Description,
                Status = createDto.Status,
                DueDate = createDto.DueDate,
                UserId = createDto.UserId,
                TodoItemListId = createDto.TodoItemListId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdItem = await _todoItemRepository.AddAsync(todoItem);
            return MapToDto(createdItem);
        }

        public async Task<TodoItemDto> UpdateAsync(int id, UpdateTodoItemDto updateDto)
        {
            var existingItem = await _todoItemRepository.GetByIdAsync(id);
            if (existingItem == null)
            {
                throw new KeyNotFoundException($"TodoItem with id {id} not found.");
            }

            existingItem.Title = updateDto.Title;
            existingItem.Description = updateDto.Description;
            existingItem.Status = updateDto.Status;
            existingItem.DueDate = updateDto.DueDate;
            existingItem.TodoItemListId = updateDto.TodoItemListId;
            existingItem.UpdatedAt = DateTime.UtcNow;

            await _todoItemRepository.UpdateAsync(existingItem);
            return MapToDto(existingItem);
        }

        public async Task DeleteAsync(int id)
        {
            await _todoItemRepository.DeleteAsync(id);
        }

        private static TodoItemDto MapToDto(TodoItem todoItem)
        {
            return new TodoItemDto
            {
                Id = todoItem.Id,
                Title = todoItem.Title,
                Description = todoItem.Description,
                Status = todoItem.Status,
                DueDate = todoItem.DueDate,
                UserId = todoItem.UserId,
                TodoItemListId = todoItem.TodoItemListId,
                CreatedAt = todoItem.CreatedAt,
                UpdatedAt = todoItem.UpdatedAt
            };
        }
    }
}