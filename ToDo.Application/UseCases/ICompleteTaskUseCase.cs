using ToDo.Application.DTOs;
public interface ICompleteTaskUseCase
{
    Task<TodoItemDto> ExecuteAsync(int taskId, string userId);
}