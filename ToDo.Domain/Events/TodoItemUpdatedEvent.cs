using ToDo.Domain.Entities;

namespace ToDo.Domain.Events
{
    public class TodoItemUpdatedEvent : DomainEvent
    {
        public TodoItem TodoItem { get; }

        public TodoItemUpdatedEvent(TodoItem todoItem) : base("TodoItemUpdated", todoItem.UserId)
        {
            TodoItem = todoItem;
        }
    }
}