# ADR: Selection of Blazor WebAssembly with Server-Side Pre-Rendering as the Frontend Framework

## Status

Accepted

## Context

Having chosen C# as our primary programming language, we need to select a frontend framework that aligns with our technology stack and project goals. We are particularly focused on maintaining a unified codebase, facilitating easier integration with Large Language Models (LLMs) for code generation, and ensuring a smooth user experience.

## Decision

We have decided to use Blazor WebAssembly (WASM) as our primary frontend framework, enhanced with Blazor Server-Side Pre-Rendering (also known as Progressive Enhancement) for improved initial load times and SEO.

## Rationale

Blazor WebAssembly with Server-Side Pre-Rendering was chosen for the following reasons:

1. Unified Technology Stack: Blazor WASM allows us to use C# for both backend and frontend development, keeping everything in the same project and codebase. This uniformity simplifies development and maintenance.

2. Reduced Line Count: By using a single language and framework across the stack, we can potentially reduce the overall line count of our codebase, making it easier for LLMs to work with and generate code for our project.

3. Modern Choice for Frontend: Blazor WASM represents one of the most modern approaches to web development within the .NET ecosystem, allowing us to build rich, interactive web UIs using C# instead of JavaScript.

4. Client-Side Execution: As a WebAssembly-based framework, Blazor WASM runs entirely in the browser, offering fast performance and reduced server load for many operations.

5. Improved Initial Load Time: By implementing Server-Side Pre-Rendering, we address the primary drawback of Blazor WASM (slow initial load) by serving a pre-rendered version of the page while the WASM bundle downloads in the background.

6. Enhanced SEO: Server-Side Pre-Rendering ensures that search engines can crawl our content effectively, improving our application's SEO potential.

7. Progressive Enhancement: This approach provides a functional application even before the WebAssembly bundle is fully loaded, enhancing the user experience on slower connections or devices.

### Alternatives Considered

1. Blazor Server (without WASM):
   - While this would provide fast initial loads, it requires a constant connection to the server, which might not be ideal for all use cases.

2. Traditional JavaScript Frameworks (React, Angular, Vue):
   - Rejected because they would introduce a separate language and ecosystem, increasing complexity and line count.

3. ASP.NET MVC with Razor Pages:
   - While this keeps us in the .NET ecosystem, it's more suited for server-rendered applications and doesn't offer the modern, interactive experience we're aiming for.

## Consequences

### Positive

- Unified codebase in C# for both frontend and backend, simplifying development and maintenance.
- Potentially lower line count, making it easier for LLMs to generate and work with our code.
- Modern, interactive user interfaces without the need to write JavaScript.
- Leverages our team's existing C# skills for frontend development.
- Improved initial page load times and SEO due to Server-Side Pre-Rendering.
- Better user experience on slow connections or devices due to Progressive Enhancement.

### Negative

- Increased complexity in the deployment and hosting setup to support both server-side rendering and WebAssembly.
- Potential for inconsistencies between server-rendered and client-rendered content if not managed carefully.

### Neutral

- Developers will need to familiarize themselves with both Blazor WASM and Server-Side Pre-Rendering patterns and best practices.
- We'll need to carefully manage state transfer between the server-rendered application and the WASM application.

## References

- Blazor WebAssembly documentation: https://docs.microsoft.com/en-us/aspnet/core/blazor/?view=aspnetcore-5.0#blazor-webassembly
- Blazor hosting models: https://docs.microsoft.com/en-us/aspnet/core/blazor/hosting-models?view=aspnetcore-5.0
- Blazor Server-Side Pre-Rendering: https://docs.microsoft.com/en-us/aspnet/core/blazor/components/prerendering-and-integration?view=aspnetcore-5.0&pivots=webassembly