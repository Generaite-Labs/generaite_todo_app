using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using ToDo.WebClient.ToDoClient;
using ToDo.WebClient.ToDoClient.Models;
using ToDo.WebClient.Auth;
using Microsoft.Kiota.Abstractions;

namespace ToDo.WebClient.Services
{
    public class AuthService
    {
        private readonly ApiClient _apiClient;
        private readonly NavigationManager _navigationManager;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly NotificationService _notificationService;

        public AuthService(
            ApiClient apiClient,
            NavigationManager navigationManager,
            AuthenticationStateProvider authStateProvider,
            NotificationService notificationService)
        {
            _apiClient = apiClient;
            _navigationManager = navigationManager;
            _authStateProvider = authStateProvider;
            _notificationService = notificationService;
        }

        public async Task<bool> LoginAsync(LoginRequest loginRequest)
        {
            try
            {
                var response = await _apiClient.Login.PostAsync(loginRequest);
                _notificationService.Notify(NotificationSeverity.Success, "Success", "Logged in successfully.");
                _navigationManager.NavigateTo("/");
                return true;
            }
            catch (ApiException ex)
            {
                // Handle API-specific errors
                var statusCode = ex.ResponseStatusCode;
                var errorMessage = ex.Message;
                _notificationService.Notify(NotificationSeverity.Error, "Error", $"Login failed. Status code: {statusCode}. Message: {errorMessage}");
                Console.Error.WriteLine($"Login error: Status code {statusCode}, Message: {errorMessage}");
                return false;
            }
            catch (Exception ex)
            {
                _notificationService.Notify(NotificationSeverity.Error, "Error", "An error occurred during login.");
                Console.Error.WriteLine($"Login error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RegisterAsync(RegisterRequest registerRequest)
        {
            try
            {
                // Attempt to make the API call
                var response = await _apiClient.Register.PostAsync(registerRequest);
                _notificationService.Notify(NotificationSeverity.Success, "Success", "Registration successful. Please log in.");
                _navigationManager.NavigateTo("/login");
                return true;
            }
            catch (ApiException ex)
            {
                // Handle API-specific errors
                var statusCode = ex.ResponseStatusCode;
                var errorMessage = ex.Message;
                _notificationService.Notify(NotificationSeverity.Error, "Error", $"Registration failed. Status code: {statusCode}. Message: {errorMessage}");
                Console.Error.WriteLine($"Registration error: Status code {statusCode}, Message: {errorMessage}");
                return false;
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                _notificationService.Notify(NotificationSeverity.Error, "Error", "An unexpected error occurred during registration.");
                Console.Error.WriteLine($"Unexpected registration error: {ex.Message}");
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                await _apiClient.Logout.PostAsync();
                ((CookieAuthenticationStateProvider)_authStateProvider).NotifyAuthenticationStateChanged();
                _notificationService.Notify(NotificationSeverity.Info, "Logged out", "You have been logged out successfully.");
                _navigationManager.NavigateTo("/");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Logout error: {ex.Message}");
                _notificationService.Notify(NotificationSeverity.Error, "Error", "An error occurred during logout.");
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            return authState.User.Identity?.IsAuthenticated ?? false;
        }
    }
}
