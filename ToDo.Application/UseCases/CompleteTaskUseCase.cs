using ToDo.Core.DTOs;
using ToDo.Application.Exceptions;
using ToDo.Application.Interfaces;
using Microsoft.Extensions.Logging;

public class CompleteTaskUseCase : ICompleteTaskUseCase
{
  private readonly ITodoItemService _todoItemService;
  private readonly ILogger<CompleteTaskUseCase> _logger;

  public CompleteTaskUseCase(ITodoItemService todoItemService, ILogger<CompleteTaskUseCase> logger)
  {
    _todoItemService = todoItemService;
    _logger = logger;
  }

  public async Task<TodoItemDto> ExecuteAsync(Guid taskId, string userId)
  {
    _logger.LogInformation("Completing task {TaskId} for user {UserId}", taskId, userId);

    var task = await _todoItemService.GetByIdAsync(userId, taskId);
    if (task == null)
    {
      throw new TodoItemNotFoundException(taskId);
    }

    if (task.Status == "Completed")
    {
      _logger.LogInformation("Task {TaskId} is already completed", taskId);
      return task;
    }

    var updateDto = new UpdateTodoItemDto
    {
      Status = "Completed",
      CompletedAt = DateTime.UtcNow
    };

    var updatedTask = await _todoItemService.UpdateAsync(userId, taskId, updateDto);

    _logger.LogInformation("Task {TaskId} completed successfully", taskId);

    return updatedTask;
  }
}
