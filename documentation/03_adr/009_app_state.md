# ADR: State Management in Blazor WebAssembly

## Status
Accepted

## Context
Our To-Do application, built with Blazor WebAssembly, requires an effective method for managing client-side state. We need a solution that balances simplicity, performance, and the potential for future scalability as our application grows in complexity.

## Decision
We will implement state management using browser storage (localStorage and sessionStorage) as our primary method, with the intention to potentially transition to IndexedDB as the application's complexity increases.

## Rationale
- Browser storage (localStorage and sessionStorage) provides a simple and built-in solution for client-side storage in web applications.
- localStorage persists data even after the browser window is closed, which is suitable for long-term state storage.
- sessionStorage is useful for temporary state that should be cleared when the session ends.
- This approach allows for quick implementation and is sufficient for our current needs.
- The Web Storage API is well-supported across modern browsers.
- Transitioning to IndexedDB in the future will allow for more complex data structures and larger storage capacity if needed.

## Implementation Details
1. Use `Blazored.LocalStorage` and `Blazored.SessionStorage` NuGet packages to simplify interactions with browser storage in Blazor.
2. Implement a state management service that interfaces with browser storage.
3. Use localStorage for persistent data (e.g., user preferences, completed tasks).
4. Use sessionStorage for temporary session data (e.g., current view state, filters).
5. Serialize complex objects to JSON before storing and deserialize when retrieving.
6. Implement a caching layer to minimize storage read/write operations.

## Consequences
### Positive
- Simple to implement and use.
- Built-in browser support with no additional dependencies.
- Sufficient for current application needs.
- Clear separation between persistent and session-based state.

### Negative
- Limited storage capacity (typically 5-10MB per origin).
- Synchronous API which could potentially block the main thread with large datasets.
- Limited query capabilities compared to more advanced solutions like IndexedDB.

## Future Considerations
- Monitor application performance and storage usage as the application grows.
- Prepare for transition to IndexedDB when more complex data structures or larger storage capacity is required.
- Consider implementing a hybrid approach, using browser storage for frequently accessed data and IndexedDB for larger datasets.
- Evaluate the need for state synchronization with server-side storage as the application evolves.

## Implementation Example
Here's a basic example of how we might implement a state management service using browser storage:

```csharp
public class StateService
{
    private readonly ILocalStorageService _localStorage;
    private readonly ISessionStorageService _sessionStorage;

    public StateService(ILocalStorageService localStorage, ISessionStorageService sessionStorage)
    {
        _localStorage = localStorage;
        _sessionStorage = sessionStorage;
    }

    public async Task SavePersistentDataAsync<T>(string key, T data)
    {
        await _localStorage.SetItemAsync(key, data);
    }

    public async Task<T> GetPersistentDataAsync<T>(string key)
    {
        return await _localStorage.GetItemAsync<T>(key);
    }

    public async Task SaveSessionDataAsync<T>(string key, T data)
    {
        await _sessionStorage.SetItemAsync(key, data);
    }

    public async Task<T> GetSessionDataAsync<T>(string key)
    {
        return await _sessionStorage.GetItemAsync<T>(key);
    }
}
```