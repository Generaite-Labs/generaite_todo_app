using ToDo.Domain.Entities;

namespace ToDo.Domain.Events
{
    public class TodoItemCreatedEvent : DomainEvent
    {
        public TodoItem TodoItem { get; }

        public TodoItemCreatedEvent(TodoItem todoItem) : base()
        {
            TodoItem = todoItem;
        }
    }
}