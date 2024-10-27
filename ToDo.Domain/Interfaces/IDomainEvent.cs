namespace ToDo.Domain.Interfaces {
    public interface IDomainEvent
    {
        Guid Id { get; }
        DateTime OccurredOn { get; }
        string EventType { get; }
        string UserId { get; }
    }
}