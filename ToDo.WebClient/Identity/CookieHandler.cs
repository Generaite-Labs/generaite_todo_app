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
            Console.WriteLine($"CookieHandler.SendAsync: Preparing request to {request.RequestUri}");
            
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            request.Headers.Add("X-Requested-With", ["XMLHttpRequest"]);

            // Log request headers
            Console.WriteLine("Request Headers:");
            foreach (var header in request.Headers)
            {
                Console.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
            }

            // Log cookies in the request
            if (request.Headers.TryGetValues("Cookie", out var cookies))
            {
                Console.WriteLine("Request Cookies:");
                foreach (var cookie in cookies)
                {
                    Console.WriteLine($"  {cookie}");
                }
            }
            else
            {
                Console.WriteLine("No cookies in the request.");
            }

            var response = await base.SendAsync(request, cancellationToken);

            Console.WriteLine($"CookieHandler.SendAsync: Received response with status code {response.StatusCode}");

            // Log response headers
            Console.WriteLine("Response Headers:");
            foreach (var header in response.Headers)
            {
                Console.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
            }

            // Log Set-Cookie headers
            if (response.Headers.TryGetValues("Set-Cookie", out var setCookies))
            {
                Console.WriteLine("Set-Cookie Headers:");
                foreach (var setCookie in setCookies)
                {
                    Console.WriteLine($"  {setCookie}");
                }
            }
            else
            {
                Console.WriteLine("No Set-Cookie headers in the response.");
            }

            return response;
        }

        public Task AuthenticateRequestAsync(RequestInformation request, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
        {
            Console.WriteLine($"CookieHandler.AuthenticateRequestAsync: Authenticating request to {request.URI}");
            
            request.Headers.Add("X-Requested-With", new[] { "XMLHttpRequest" });

            // Log request headers
            Console.WriteLine("Request Headers:");
            foreach (var header in request.Headers)
            {
                Console.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
            }

            return Task.CompletedTask;
        }
    }
}
