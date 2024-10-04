using ToDo.Application.DTOs;
using ToDo.Application.Exceptions;
using ToDo.Domain.Entities;
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

    public async Task<TodoItemDto> ExecuteAsync(int taskId, string userId)
    {
        _logger.LogInformation("Completing task {TaskId} for user {UserId}", taskId, userId);

        var task = await _todoItemService.GetByIdAsync(userId, taskId);
        if (task == null)
        {
            throw new TodoItemNotFoundException(taskId);
        }

        if (task.Status == TodoItemStatusDto.COMPLETED)
        {
            _logger.LogInformation("Task {TaskId} is already completed", taskId);
            return task;
        }

        var updateDto = new UpdateTodoItemDto
        {
            Status = TodoItemStatusDto.COMPLETED,
            CompletedAt = DateTime.UtcNow
        };

        var updatedTask = await _todoItemService.UpdateAsync(userId, taskId, updateDto);

        _logger.LogInformation("Task {TaskId} completed successfully", taskId);

        return updatedTask;
    }
}