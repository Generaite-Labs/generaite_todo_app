# ADR: Using Fluent Validation for Input Validation

## Status
Accepted

## Context
Our To-Do application requires robust input validation to ensure data integrity and improve user experience. We need a validation solution that is flexible, easy to implement, and integrates well with our C# and ASP.NET Core backend, as well as our Blazor WebAssembly frontend.

## Decision
We will use FluentValidation library for implementing validation logic in our application.

## Rationale
- FluentValidation provides a fluent interface for defining strongly-typed validation rules.
- It separates validation logic from model classes, promoting cleaner and more maintainable code.
- FluentValidation integrates well with ASP.NET Core for server-side validation.
- It can be used in Blazor WebAssembly for client-side validation, ensuring a consistent validation approach across our stack.
- The library is highly customizable and extensible, allowing for complex validation scenarios.
- It supports localization, which may be beneficial for future internationalization of our application.

## Implementation Details
1. Add the FluentValidation NuGet package to both the server and client projects.
2. Create validator classes for each DTO or model that requires validation.
3. Register validators with the dependency injection container.
4. Integrate FluentValidation with ASP.NET Core's model binding for automatic server-side validation.
5. Implement client-side validation in Blazor components using FluentValidation.

## Consequences
### Positive
- Consistent validation logic across server and client.
- Improved code readability and maintainability through separation of concerns.
- Strongly-typed validation rules reduce the likelihood of errors.
- Extensibility allows for custom validation rules as needed.

### Negative
- Additional dependency in the project.
- Learning curve for team members unfamiliar with FluentValidation.
- Potential for duplication of validation logic if not carefully managed between server and client.

## Future Considerations
- Evaluate the need for custom validation rules as the application grows.
- Consider implementing a validation message localization strategy for multi-language support.
- Monitor the performance impact of client-side validation in Blazor WebAssembly and optimize if necessary.

## Implementation Example
Here's a basic example of how we might implement a validator for a Task DTO:

```csharp
public class TaskDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime? DueDate { get; set; }
}

public class TaskDtoValidator : AbstractValidator<TaskDto>
{
    public TaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.DueDate)
            .GreaterThanOrEqualTo(DateTime.Today)
            .When(x => x.DueDate.HasValue)
            .WithMessage("Due date must be today or in the future.");
    }
}
```