using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ToDo.WebClient;
using ToDo.WebClient.Auth;
using Blazored.LocalStorage;
using Radzen;
using ToDo.Core.Configuration;
using ToDo.WebClient.ToDoClient;
using Microsoft.AspNetCore.Components.Authorization;
using ToDo.WebClient.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Build configuration
var configuration = ApplicationConfig.BuildConfiguration();
builder.Services.AddApplicationConfiguration(configuration);

// Add cookie handler
builder.Services.AddScoped<CookieHandler>();

builder.Services.AddHttpClient<ToDoClientFactory>((sp, client) => 
{
    var appSettings = sp.GetRequiredService<ApplicationSettings>();
    client.BaseAddress = new Uri(appSettings.ApiBaseUrl);
}).AddHttpMessageHandler<CookieHandler>();

// Register the ToDo client
builder.Services.AddScoped<ApiClient>(sp => sp.GetRequiredService<ToDoClientFactory>().GetClient());

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddRadzenComponents();

// Add authentication state provider
builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthService>();

await builder.Build().RunAsync();
