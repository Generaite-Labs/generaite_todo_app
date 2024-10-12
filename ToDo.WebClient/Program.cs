using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ToDo.WebClient;
using ToDo.WebClient.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using ToDo.WebClient.Auth;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Configure HttpClient for AuthenticationService
builder.Services.AddHttpClient<AuthenticationService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7197/");
});

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

await builder.Build().RunAsync();
