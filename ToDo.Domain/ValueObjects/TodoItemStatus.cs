using System;

namespace ToDo.Domain.ValueObjects
{
  public class TodoItemStatus : IEquatable<TodoItemStatus>
  {
    public static TodoItemStatus NotStarted => new TodoItemStatus(nameof(NotStarted));
    public static TodoItemStatus InProgress => new TodoItemStatus(nameof(InProgress));
    public static TodoItemStatus Completed => new TodoItemStatus(nameof(Completed));

    public string Value { get; }

    private TodoItemStatus(string value)
    {
      Value = value;
    }

    public static TodoItemStatus FromString(string status)
    {
      return status.ToLower() switch
      {
        "notstarted" => NotStarted,
        "inprogress" => InProgress,
        "completed" => Completed,
        _ => throw new ArgumentException($"Invalid status: {status}", nameof(status))
      };
    }

    public bool Equals(TodoItemStatus? other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != this.GetType()) return false;
      return Equals((TodoItemStatus)obj);
    }

    public override int GetHashCode()
    {
      return StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
    }

    public static bool operator ==(TodoItemStatus? left, TodoItemStatus? right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(TodoItemStatus? left, TodoItemStatus? right)
    {
      return !Equals(left, right);
    }

    public override string ToString() => Value;

    public bool IsCompleted() => this == Completed;
    public bool IsInProgress() => this == InProgress;
    public bool IsNotStarted() => this == NotStarted;
  }
}
