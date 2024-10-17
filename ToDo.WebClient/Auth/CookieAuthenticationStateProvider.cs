using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace ToDo.WebClient.Auth;
public class CookieAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;

    public CookieAuthenticationStateProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var userClaims = await GetUserClaimsAsync();
        var identity = userClaims != null && userClaims.Any()
            ? new ClaimsIdentity(userClaims, "serverauth")
            : new ClaimsIdentity();

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    private async Task<IEnumerable<Claim>?> GetUserClaimsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Claim>>("api/auth/user");
        }
        catch
        {
            return null;
        }
    }

    public void NotifyAuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
