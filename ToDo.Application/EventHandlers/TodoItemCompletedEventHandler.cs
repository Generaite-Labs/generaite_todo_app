using ToDo.Domain.Events;
using ToDo.Domain.Interfaces;

namespace ToDo.Application.EventHandlers
{
    public class TodoItemCompletedEventHandler : IDomainEventHandler<TodoItemCompletedEvent>
    {
        public async Task HandleAsync(TodoItemCompletedEvent domainEvent)
        {
            // Handle the event, e.g., send a notification, update statistics, etc.
            Console.WriteLine($"Todo item {domainEvent.TodoItemId} was completed at {domainEvent.OccurredOn}");
            await Task.CompletedTask;
        }
    }
}