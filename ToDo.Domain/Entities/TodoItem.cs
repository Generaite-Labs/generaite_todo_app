using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ToDo.Domain.Events;
using ToDo.Domain.ValueObjects;

namespace ToDo.Domain.Entities
{
  public class TodoItem
  {
    public Guid Id { get; init; }

    [Required]
    public required string Title { get; set; }

    public string? Description { get; private set; }

    public required TodoItemStatus Status { get; set; } // Remove the '?' to make it non-nullable

    public DateTime? DueDate { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public DateTime? StartedAt { get; private set; }

    public required string UserId { get; set; }

    public string? AssignedUserId { get; private set; }

    [ForeignKey("UserId")]
    public virtual ApplicationUser? User { get; init; }

    [ForeignKey("AssignedUserId")]
    public virtual ApplicationUser? AssignedUser { get; private set; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; private set; }

    private readonly List<DomainEvent> _domainEvents = new List<DomainEvent>();
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private TodoItem() { }

    public static TodoItem CreateTodoItem(string title, string? description, string userId, DateTime? dueDate)
    {
      if (string.IsNullOrWhiteSpace(title))
        throw new ArgumentException("Title cannot be empty", nameof(title));

      var todoItem = new TodoItem
      {
        Title = title,
        Description = description,
        UserId = userId,
        DueDate = dueDate?.ToUniversalTime(), // Ensure DueDate is in UTC
        Status = TodoItemStatus.NotStarted,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
      };

      todoItem._domainEvents.Add(new TodoItemCreatedEvent(todoItem));
      return todoItem;
    }

    public void StartTodoItem()
    {
      Status = TodoItemStatus.InProgress;
      StartedAt = DateTime.UtcNow;
      CompletedAt = null;
      UpdatedAt = DateTime.UtcNow;
    }

    public void StopTodoItem()
    {
      Status = TodoItemStatus.NotStarted;
      CompletedAt = null;
      UpdatedAt = DateTime.UtcNow;
    }

    public void CompleteTodoItem()
    {
      Status = TodoItemStatus.Completed;
      CompletedAt = DateTime.UtcNow;
      UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTodoItem(string title, string? description, DateTime? dueDate)
    {
      if (string.IsNullOrWhiteSpace(title))
        throw new ArgumentException("Title cannot be empty", nameof(title));

      Title = title;
      Description = description;
      DueDate = dueDate?.ToUniversalTime(); // Ensure DueDate is in UTC
      UpdatedAt = DateTime.UtcNow;
      _domainEvents.Add(new TodoItemUpdatedEvent(this));
    }

    public void AssignTodoItem(string assignedUserId)
    {
      if (string.IsNullOrWhiteSpace(assignedUserId))
        throw new ArgumentException("Assigned user ID cannot be empty", nameof(assignedUserId));

      AssignedUserId = assignedUserId;
      UpdatedAt = DateTime.UtcNow;
      _domainEvents.Add(new TodoItemUpdatedEvent(this));
    }

    public void ClearDomainEvents()
    {
      _domainEvents.Clear();
    }

    public void MarkAsCompleted()
    {
      if (Status != TodoItemStatus.Completed)
      {
        Status = TodoItemStatus.Completed;
        _domainEvents.Add(new TodoItemUpdatedEvent(this));
      }
    }
  }
}
