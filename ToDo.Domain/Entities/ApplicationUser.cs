using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace ToDo.Domain.Entities
{
  public class ApplicationUser : IdentityUser
  {
    public required string FullName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public virtual ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
  }
}