namespace ToDo.Domain.Events
{
    public class TodoItemCompletedEvent : DomainEvent
    {
        public int TodoItemId { get; }

        public TodoItemCompletedEvent(int todoItemId)
        {
            TodoItemId = todoItemId;
        }
    }
}