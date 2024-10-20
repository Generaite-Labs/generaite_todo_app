using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using ToDo.WebClient.ToDoClient;
using ToDo.WebClient.ToDoClient.Models;
using ToDo.WebClient.Identity.Models;
using Microsoft.Kiota.Abstractions;

namespace ToDo.WebClient.Identity;

public class CookieAuthenticationStateProvider : AuthenticationStateProvider, IAccountManagement
{
    private readonly ApiClient _apiClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private bool _authenticated = false;
    private readonly ClaimsPrincipal _unauthenticated = new(new ClaimsIdentity());

    public CookieAuthenticationStateProvider(ApiClient apiClient)
    {
        _apiClient = apiClient;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        Console.WriteLine("CookieAuthenticationStateProvider: GetAuthenticationStateAsync called");
        _authenticated = false;
        var user = _unauthenticated;

        try
        {
            Console.WriteLine("CookieAuthenticationStateProvider: Attempting to get user info");
            var userInfo = await _apiClient.Api.Auth.User.GetAsync();
            Console.WriteLine($"CookieAuthenticationStateProvider: User info received. IsAuthenticated: {userInfo.IsAuthenticated}");
            if (userInfo.IsAuthenticated ?? false)
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, userInfo.Email),
                    new(ClaimTypes.Email, userInfo.Email),
                };

                var id = new ClaimsIdentity(claims, nameof(CookieAuthenticationStateProvider));
                user = new ClaimsPrincipal(id);
                _authenticated = true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CookieAuthenticationStateProvider: Exception occurred: {ex.Message}");
            // If an exception occurs, we assume the user is not authenticated
        }

        return new AuthenticationState(user);
    }

    public async Task<FormResult> LoginAsync(string email, string password)
    {
        Console.WriteLine("CookieAuthenticationStateProvider: LoginAsync called");
        try
        {
            Console.WriteLine($"CookieAuthenticationStateProvider: Attempting to login with email: {email}");
            var loginRequest = new LoginRequest
            {
                Email = email,
                Password = password
            };

            var response = await _apiClient.Login.PostAsync(loginRequest, q => q.QueryParameters.UseCookies = true);

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            Console.WriteLine("CookieAuthenticationStateProvider: Authentication state changed");
            return new FormResult { Succeeded = true };
        }
        catch (ApiException apiException)
        {
            Console.WriteLine($"CookieAuthenticationStateProvider: Login failed: {apiException.Message}");
            Console.WriteLine($"ApiException details: {apiException}");
            return new FormResult
            {
                Succeeded = false,
                ErrorList = new[] { $"Login failed: {apiException.Message}" }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CookieAuthenticationStateProvider: Exception occurred: {ex.Message}");
            Console.WriteLine($"Exception details: {ex}");
            return new FormResult
            {
                Succeeded = false,
                ErrorList = new[] { $"An unexpected error occurred: {ex.Message}" }
            };
        }
    }

    public async Task LogoutAsync()
    {
        await _apiClient.Logout.PostAsync();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task<bool> CheckAuthenticatedAsync()
    {
        await GetAuthenticationStateAsync();
        return _authenticated;
    }

    public async Task<FormResult> RegisterAsync(string email, string password)
    {
        try
        {
            var registerRequest = new RegisterRequest
            {
                Email = email,
                Password = password
            };

            await _apiClient.Register.PostAsync(registerRequest);
            return new FormResult { Succeeded = true };
        }
        catch (ApiException apiException)
        {
            return new FormResult
            {
                Succeeded = false,
                ErrorList = new[] { $"An unexpected error occurred: {apiException.Message}" }
            };
        }
        catch (Exception ex)
        {
            return new FormResult
            {
                Succeeded = false,
                ErrorList = new[] { $"An unexpected error occurred: {ex.Message}" }
            };
        }
    }

    // Implement other IAccountManagement methods as needed
}
