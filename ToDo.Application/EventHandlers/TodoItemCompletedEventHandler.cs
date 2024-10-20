using ToDo.Domain.Events;
using ToDo.Domain.Interfaces;

namespace ToDo.Application.EventHandlers
{
    public class TodoItemCompletedEventHandler : IDomainEventHandler<TodoItemCompletedEvent>
    {
        public async Task HandleAsync(TodoItemCompletedEvent domainEvent)
        {
            await Task.CompletedTask;
        }
    }
}