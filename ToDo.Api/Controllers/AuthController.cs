using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    [HttpGet("user")]
    public ActionResult<UserInfo> GetUserInfo()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return new UserInfo
            {
                IsAuthenticated = true,
                UserName = User.Identity.Name ?? string.Empty,
                Email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
                Id = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty,
                ExposedClaims = User.Claims.ToDictionary(c => c.Type, c => c.Value)
            };
        }
        return new UserInfo { IsAuthenticated = false };
    }
}

public class UserInfo
{
    public bool IsAuthenticated { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public Dictionary<string, string> ExposedClaims { get; set; } = new Dictionary<string, string>();
}
