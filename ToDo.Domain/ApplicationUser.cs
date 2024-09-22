﻿using Microsoft.AspNetCore.Identity;

namespace ToDo.Domain.Entities
{
  public class ApplicationUser : IdentityUser
  {
    public required string FullName { get; set; }
  }

}