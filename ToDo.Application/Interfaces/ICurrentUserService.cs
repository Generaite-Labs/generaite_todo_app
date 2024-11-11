using ToDo.Domain.Entities;

namespace ToDo.Application.Interfaces;

public interface ICurrentUserService
{
    Task<ApplicationUser?> GetCurrentUser();
} 