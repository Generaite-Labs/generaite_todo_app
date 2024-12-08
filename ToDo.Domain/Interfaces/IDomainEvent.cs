namespace ToDo.Domain.Interfaces
{
  public interface IDomainEvent
  {
    Guid Id { get; }
    DateTime OccurredOn { get; }
    long Version { get; }
    Guid AggregateId { get; }
    string AggregateType { get; }
  }
}
