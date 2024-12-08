using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ToDo.WebClient;
using ToDo.WebClient.Identity;
using Blazored.LocalStorage;
using Radzen;
using ToDo.Core.Configuration;
using ToDo.WebClient.ToDoClient;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Build configuration
var configuration = ApplicationConfig.BuildConfiguration();
builder.Services.AddApplicationConfiguration(configuration);

// Register CookieHandler
builder.Services.AddScoped<CookieHandler>();

// Update the ToDoClientFactory registration
builder.Services.AddScoped<ToDoClientFactory>();

// Replace the existing HttpClient registration with this:
builder.Services.AddHttpClient("API", client =>
{
  var appSettings = builder.Services.BuildServiceProvider().GetRequiredService<ApplicationSettings>();
  client.BaseAddress = new Uri(appSettings.ApiBaseUrl);
});

// Keep the existing ApiClient registration
builder.Services.AddScoped<ApiClient>(sp =>
{
  var factory = sp.GetRequiredService<ToDoClientFactory>();
  return factory.GetClient();
});

builder.Services.AddRadzenComponents();

// Set up authorization
builder.Services.AddAuthorizationCore();

// Register the custom state provider
builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();

// Register the account management interface
builder.Services.AddScoped(
    sp => (IAccountManagement)sp.GetRequiredService<AuthenticationStateProvider>());

await builder.Build().RunAsync();
