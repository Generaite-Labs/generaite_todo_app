using ToDo.Domain.Entities;

namespace ToDo.Domain.Interfaces
{
    public interface IDomainEventService
    {
        Task PublishEventsAsync(IEnumerable<TodoItem> entities);
    }
}