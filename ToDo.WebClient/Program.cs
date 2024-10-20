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

// Register ToDoClientFactory
builder.Services.AddHttpClient<ToDoClientFactory>((sp, client) => 
{
    var appSettings = sp.GetRequiredService<ApplicationSettings>();
    client.BaseAddress = new Uri(appSettings.ApiBaseUrl);
});

// Register ApiClient using the factory
builder.Services.AddScoped<ApiClient>(sp => 
{
    var factory = sp.GetRequiredService<ToDoClientFactory>();
    return factory.GetClient();
});

builder.Services.AddRadzenComponents();

// register the cookie handler
builder.Services.AddTransient<CookieHandler>();

// set up authorization
builder.Services.AddAuthorizationCore();

// register the custom state provider
builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();

// register the account management interface
builder.Services.AddScoped(
    sp => (IAccountManagement)sp.GetRequiredService<AuthenticationStateProvider>());

await builder.Build().RunAsync();
