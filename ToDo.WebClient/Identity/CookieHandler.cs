using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Abstractions;
using System.Net.Http.Headers;

namespace ToDo.WebClient.Identity
{
  public class CookieHandler : DelegatingHandler, IAuthenticationProvider
  {
    public CookieHandler()
    {
      InnerHandler = new HttpClientHandler();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
      request.Headers.Add("X-Requested-With", ["XMLHttpRequest"]);
      var response = await base.SendAsync(request, cancellationToken);
      return response;
    }

    public Task AuthenticateRequestAsync(RequestInformation request, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
    {
      request.Headers.Add("X-Requested-With", new[] { "XMLHttpRequest" });
      return Task.CompletedTask;
    }
  }
}
