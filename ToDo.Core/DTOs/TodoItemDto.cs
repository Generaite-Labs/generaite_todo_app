namespace ToDo.Core.DTOs
{
    public class TodoItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Status { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateTodoItemDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Status { get; set; }
        public DateTime? DueDate { get; set; }
        public string UserId { get; set; } = string.Empty;
    }

    public class UpdateTodoItemDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Status { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? DueDate { get; set; }
    }
}