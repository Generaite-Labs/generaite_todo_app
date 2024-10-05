using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ToDo.Domain.Events;  // Add this using statement

namespace ToDo.Domain.Entities
{
  public class TodoItem
  {
    public int Id { get; init; }

    [Required]
    public required string Title { get; set; }

    public string? Description { get; private set; }

    public TodoItemStatus Status { get; private set; }

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
        DueDate = dueDate,
        Status = TodoItemStatus.TODO,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
      };

      todoItem._domainEvents.Add(new TodoItemCreatedEvent(todoItem));
      return todoItem;
    }

    public void StartTodoItem()
    {
      Status = TodoItemStatus.IN_PROGRESS;
      StartedAt = DateTime.UtcNow;
      CompletedAt = null;
      UpdatedAt = DateTime.UtcNow;
    }

    public void StopTodoItem()
    {
      Status = TodoItemStatus.TODO;
      CompletedAt = null;
      UpdatedAt = DateTime.UtcNow;
    }

    public void CompleteTodoItem()
    {
      Status = TodoItemStatus.COMPLETED;
      CompletedAt = DateTime.UtcNow;
      UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTodoItem(string title, string? description, DateTime? dueDate)
    {
      if (string.IsNullOrWhiteSpace(title))
        throw new ArgumentException("Title cannot be empty", nameof(title));

      Title = title;
      Description = description;
      DueDate = dueDate;
      UpdatedAt = DateTime.UtcNow;
      _domainEvents.Add(new TodoItemUpdatedEvent(this));
    }

    public void AssignTodoItem(string assignedUserId)
    {
      if (string.IsNullOrWhiteSpace(assignedUserId))
        throw new ArgumentException("Assigned user ID cannot be empty", nameof(assignedUserId));

      AssignedUserId = assignedUserId;
      UpdatedAt = DateTime.UtcNow;
      _domainEvents.Add(new TodoItemAssignedEvent(this, assignedUserId));
    }

    public void ClearDomainEvents()
    {
      _domainEvents.Clear();
    }
  }
}