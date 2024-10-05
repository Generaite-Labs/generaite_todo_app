using ToDo.Domain.Entities;

namespace ToDo.Domain.Events
{
    public class TodoItemAssignedEvent : DomainEvent
    {
        public TodoItem TodoItem { get; }
        public string AssignedUserId { get; }

        public TodoItemAssignedEvent(TodoItem todoItem, string assignedUserId) : base()
        {
            TodoItem = todoItem;
            AssignedUserId = assignedUserId;
        }
    }
}