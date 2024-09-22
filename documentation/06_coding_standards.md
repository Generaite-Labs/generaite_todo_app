# LLM-Optimized Coding Standards for To-Do Application

## 1. C# Conventions
- Use C# version 8.0 features.
- Naming:
  - Classes, Methods, Properties: PascalCase
  - Variables, Parameters: camelCase
  - Interfaces: Prefix with 'I' (e.g., IOrderService)
- Method length: Maximum 20 lines.
- Use async/await for all asynchronous operations.
- XML comments format:
  ```csharp
  /// <summary>
  /// [Method description]
  /// </summary>
  /// <param name="paramName">[Parameter description]</param>
  /// <returns>[Return value description]</returns>
  ```

## 2. Domain-Driven Design (DDD) Implementation
- Solution structure:
  - [ProjectName].Domain
  - [ProjectName].Application
  - [ProjectName].Infrastructure
  - [ProjectName].WebUI
- Domain Layer:
  - Entities: public class with Id property
  - Value Objects: record type, implement equality members
  - Aggregates: class with private collection, public methods for modification
  - Domain Events: class inheriting from INotification
- Application Layer:
  - Services: class name ending with 'Service'
  - DTOs: class name ending with 'Dto'
- Infrastructure Layer:
  - DbContext: class ending with 'DbContext'
  - Configurations: class ending with 'Configuration'
- Presentation Layer (Blazor):
  - Components: class ending with 'Component'
  - Pages: class ending with 'Page'

## 3. Entity Framework Core
- Use Code-First approach.
- Migrations: One file per significant change.
- Queries: Use LINQ, avoid raw SQL.
- Loading related data: Use Include() for eager loading.
- Index creation:
  ```csharp
  modelBuilder.Entity<EntityName>()
      .HasIndex(e => e.PropertyName);
  ```

## 4. Blazor WebAssembly
- Component structure:
  ```csharp
  @inherits ComponentBase
  
  <div>
      <!-- Component content -->
  </div>
  
  @code {
      // Component logic
  }
  ```
- State management: Use browser storage (localStorage/sessionStorage).
- Dependency injection: Use @inject directive.

## 5. API Design
- Endpoint naming: `/api/[resource]/[action]`
- HTTP methods:
  - GET: Retrieve
  - POST: Create
  - PUT: Update (full)
  - PATCH: Update (partial)
  - DELETE: Remove
- Response codes:
  - 200: Success
  - 201: Created
  - 400: Bad Request
  - 401: Unauthorized
  - 404: Not Found
  - 500: Server Error

## 6. Authentication
- Use ASP.NET Core Identity.
- JWT structure:
  ```json
  {
    "sub": "[UserId]",
    "jti": "[UniqueTokenId]",
    "iat": "[IssuedAt]",
    "exp": "[ExpirationTime]",
    "roles": ["Role1", "Role2"]
  }
  ```

## 7. Styling
- Use Tailwind CSS utility classes.
- DaisyUI component usage:
  ```html
  <button class="btn btn-primary">Click me</button>
  ```

## 8. Logging
- Use Serilog.
- Log levels: Debug, Information, Warning, Error
- Log format:
  ```
  {Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}
  ```

## 9. Testing
- Test naming: `[MethodName]_[Scenario]_[ExpectedResult]`
- Use xUnit attributes:
  - [Fact] for simple tests
  - [Theory] with [InlineData] for parameterized tests
- Mocking: Use Moq framework
- Blazor testing: Use bUnit
- BDD: Use SpecFlow for feature files

## 10. Validation
- Use FluentValidation.
- Validator naming: `[EntityName]Validator`
- Rule structure:
  ```csharp
  RuleFor(x => x.PropertyName)
      .NotEmpty()
      .MaximumLength(100);
  ```

## 11. Performance
- Use async methods consistently.
- Implement caching for frequently accessed, rarely changed data.
- Optimize EF Core queries:
  - Use `.AsNoTracking()` for read-only queries
  - Use `.Select()` to limit returned fields

## 12. Security
- HTTPS: Required for all communications.
- Input validation: Server-side and client-side.
- CORS policy:
  ```csharp
  services.AddCors(options =>
  {
      options.AddPolicy("AllowSpecificOrigin",
          builder => builder.WithOrigins("https://allowedorigin.com")
                            .AllowAnyMethod()
                            .AllowAnyHeader());
  });
  ```

## 13. Code Generation Directives
- Generate separate files for each DDD concept (Entity, Value Object, Aggregate).
- Include XML comments for public methods and properties.
- Generate unit tests alongside domain logic.
- For Blazor components, include both the component and a corresponding test file.

## 14. Documentation
- API documentation: Use Swagger/OpenAPI.
- Architecture overview: Maintain in solution root as 'Architecture.md'.
- Domain concepts: Document in '[ProjectName].Domain/DomainConcepts.md'.