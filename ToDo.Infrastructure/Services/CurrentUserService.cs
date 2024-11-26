using Microsoft.AspNetCore.Http;
using ToDo.Application.Interfaces;
using System.Security.Claims;
using ToDo.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ToDo.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public async Task<ApplicationUser?> GetCurrentUser()
        {
            if (UserId == null) return null;
            return await _userManager.FindByIdAsync(UserId);
        }
    }
}