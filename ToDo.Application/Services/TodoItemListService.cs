using ToDo.Domain.Entities;
using ToDo.Domain.Interfaces;
using ToDo.Application.DTOs;

namespace ToDo.Application.Services
{
    public class TodoItemListService : ITodoItemListService
    {
        private readonly ITodoItemListRepository _todoItemListRepository;

        public TodoItemListService(ITodoItemListRepository todoItemListRepository)
        {
            _todoItemListRepository = todoItemListRepository;
        }

        public async Task<TodoItemListDto?> GetByIdAsync(int id)
        {
            var todoItemList = await _todoItemListRepository.GetByIdAsync(id);
            return todoItemList != null ? MapToDto(todoItemList) : null;
        }

        public async Task<IEnumerable<TodoItemListDto>> GetAllAsync()
        {
            var todoItemLists = await _todoItemListRepository.GetAllAsync();
            return todoItemLists.Select(MapToDto);
        }

        public async Task<IEnumerable<TodoItemListDto>> GetByUserIdAsync(string userId)
        {
            var todoItemLists = await _todoItemListRepository.GetByUserIdAsync(userId);
            return todoItemLists.Select(MapToDto);
        }

        public async Task<TodoItemListDto> CreateAsync(CreateTodoItemListDto createDto)
        {
            var todoItemList = new TodoItemList
            {
                Name = createDto.Name,
                UserId = createDto.UserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdList = await _todoItemListRepository.AddAsync(todoItemList);
            return MapToDto(createdList);
        }

        public async Task<TodoItemListDto> UpdateAsync(UpdateTodoItemListDto updateDto)
        {
            var existingList = await _todoItemListRepository.GetByIdAsync(updateDto.Id);
            if (existingList == null)
            {
                throw new ArgumentException($"TodoItemList with id {updateDto.Id} not found");
            }

            existingList.Name = updateDto.Name;
            existingList.UpdatedAt = DateTime.UtcNow;

            await _todoItemListRepository.UpdateAsync(existingList);
            return MapToDto(existingList);
        }

        public async Task DeleteAsync(int id)
        {
            await _todoItemListRepository.DeleteAsync(id);
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