using System.Net.Http.Json;
using System.Net.Http;
using ToDo.Application.DTOs;

namespace ToDo.WebClient.Services
{
    public class AuthenticationService
    {
        private readonly HttpClient _httpClient;

        public AuthenticationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> Register(RegisterDto model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/account/register", model);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Registration failed. Status: {response.StatusCode}, Error: {errorContent}");
                }
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"An error occurred while registering: {ex.Message}");
                return false;
            }
        }

        public async Task<string> Login(LoginDto model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/account/login", model);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResult>();
                if (result?.Token == null)
                {
                    throw new InvalidOperationException("Login was successful but no token was received.");
                }
                return result.Token;
            }
            return null!;
        }
    }

    public class LoginResult
    {
        public required string Token { get; set; }
    }
}
