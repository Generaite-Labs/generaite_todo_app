using ToDo.Core.DTOs;
public interface ICompleteTaskUseCase
{
    Task<TodoItemDto> ExecuteAsync(int taskId, string userId);
}