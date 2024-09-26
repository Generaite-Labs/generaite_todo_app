using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace ToDo.Domain.Entities
{
  public class TodoItemList
  {
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; }

    public required string UserId { get; set; }  // Changed from int to string

    [ForeignKey("UserId")]
    public virtual ApplicationUser? User { get; set; }

    public virtual ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
  }
}