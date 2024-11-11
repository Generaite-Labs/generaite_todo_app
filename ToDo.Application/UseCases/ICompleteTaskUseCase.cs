using ToDo.Core.DTOs;
public interface ICompleteTaskUseCase
{
    Task<TodoItemDto> ExecuteAsync(Guid taskId, string userId);
}