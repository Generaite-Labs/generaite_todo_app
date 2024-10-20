using ToDo.WebClient.ToDoClient;
using ToDo.WebClient.Identity;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Microsoft.Kiota.Abstractions.Authentication;

namespace ToDo.WebClient;

public class ToDoClientFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly CookieHandler _cookieHandler;

    public ToDoClientFactory(IHttpClientFactory httpClientFactory, CookieHandler cookieHandler)
    {
        _httpClientFactory = httpClientFactory;
        _cookieHandler = cookieHandler;
    }

    public ApiClient GetClient()
    {
        var client = _httpClientFactory.CreateClient("API");
        
        // Create a new HttpMessageHandler chain
        var innerHandler = new HttpClientHandler();
        var handlerChain = new CookieHandler { InnerHandler = innerHandler };

        // Create a new HttpClient with the handler chain
        var httpClient = new HttpClient(handlerChain)
        {
            BaseAddress = client.BaseAddress
        };

        var adapter = new HttpClientRequestAdapter(_cookieHandler, httpClient: httpClient);
        adapter.BaseUrl = httpClient.BaseAddress?.ToString() ?? string.Empty;
        return new ApiClient(adapter);
    }
}
