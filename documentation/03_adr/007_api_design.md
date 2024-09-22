# ADR: Selection of REST API with ASP.NET Core Web API and SignalR for Backend Communication

## Status

Accepted

## Context

Our To-Do application, built with a Domain-Driven Design approach, requires both standard HTTP-based communication and real-time updates between the Blazor WebAssembly frontend and the backend services. We need to decide on an API architecture that supports both traditional request-response patterns and real-time communication.

## Decision

We have decided to implement:
1. A REST API using ASP.NET Core Web API for standard HTTP-based communication.
2. SignalR for real-time communication between the client and server.

## Rationale

The decision to use both REST API with ASP.NET Core Web API and SignalR is based on the following reasons:

1. Comprehensive Communication: REST API handles standard CRUD operations and data queries, while SignalR enables real-time updates and notifications.

2. Compatibility with Tech Stack: Both ASP.NET Core Web API and SignalR integrate seamlessly with our chosen C# backend and Blazor WebAssembly frontend.

3. Real-Time Capabilities: SignalR provides efficient real-time communication, enabling features like instant task updates, notifications, and collaborative features.

4. Scalability: Both technologies are designed to scale well, with SignalR supporting various backplanes for distributed applications.

5. Performance: ASP.NET Core Web API offers high performance for HTTP requests, while SignalR optimizes real-time communication.

6. Blazor WebAssembly Integration: Both REST APIs and SignalR are well-supported in Blazor WebAssembly, allowing for a rich, responsive user experience.

7. Flexibility: This combination allows us to choose the most appropriate communication method for each feature of our application.

### Implementation Overview

Our implementation will include:

1. REST API (as previously described):
   - Controllers for resource endpoints
   - DTOs for data transfer
   - Standard HTTP methods (GET, POST, PUT, DELETE, etc.)

2. SignalR Implementation:
   - Hub classes for real-time communication
   - Real-time event handling for instant updates

Example SignalR Hub:

```csharp
public class TaskHub : Hub
{
    private readonly ITaskService _taskService;

    public TaskHub(ITaskService taskService)
    {
        _taskService = taskService;
    }

    public async Task UpdateTaskStatus(Guid taskId, TaskStatus newStatus)
    {
        await _taskService.UpdateTaskStatusAsync(taskId, newStatus);
        await Clients.All.SendAsync("TaskStatusUpdated", taskId, newStatus);
    }

    public async Task NotifyTaskAssigned(Guid taskId, string assignedTo)
    {
        await Clients.User(assignedTo).SendAsync("TaskAssigned", taskId);
    }
}
```

Integration in Startup.cs:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Existing services...
    services.AddSignalR();
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    // Existing configuration...
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        endpoints.MapHub<TaskHub>("/taskHub");
    });
}
```

## Alternatives Considered

1. WebSockets without SignalR:
   - Offers low-level control but requires more complex implementation and doesn't provide the high-level features of SignalR.

2. Server-Sent Events (SSE):
   - Provides real-time server-to-client communication but lacks the full-duplex capability of SignalR.

3. Long Polling:
   - Less efficient and more resource-intensive compared to SignalR's real-time communication.

## Consequences

### Positive

- Comprehensive communication solution covering both standard and real-time scenarios.
- Enhanced user experience with instant updates and notifications.
- Scalable architecture supporting both traditional and real-time communication patterns.
- Leverages .NET ecosystem strengths in both REST API and real-time communications.

### Negative

- Increased complexity in backend implementation and state management.
- Need for careful consideration of when to use REST vs SignalR for different operations.

### Neutral

- Requires understanding and managing two different communication paradigms.
- May necessitate additional frontend logic to handle both REST calls and SignalR events.

## References

- ASP.NET Core Web API documentation: https://docs.microsoft.com/en-us/aspnet/core/web-api
- SignalR for ASP.NET Core: https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction
- Real-time ASP.NET with SignalR: https://dotnet.microsoft.com/apps/aspnet/signalr
- Using SignalR with Blazor WebAssembly: https://docs.microsoft.com/en-us/aspnet/core/blazor/tutorials/signalr-blazor