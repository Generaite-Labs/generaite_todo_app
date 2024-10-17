using ToDo.WebClient.ToDoClient;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Microsoft.Kiota.Abstractions.Authentication;

namespace ToDo.WebClient;

public class ToDoClientFactory
{
    private readonly HttpClient _httpClient;

    public ToDoClientFactory(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public ApiClient GetClient()
    {
        var authProvider = new AnonymousAuthenticationProvider();
        var adapter = new HttpClientRequestAdapter(authProvider, httpClient: _httpClient);
        return new ApiClient(adapter);
    }
}
