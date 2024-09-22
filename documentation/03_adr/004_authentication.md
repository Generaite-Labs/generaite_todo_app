# ADR: Selection of ASP.NET Core Identity for Authentication

## Status

Accepted

## Context

Our application requires a robust authentication system. Given our technology stack (C#, ASP.NET Core, Blazor) and our expectation that the application won't require enterprise-scale authentication services, we need a solution that is both integrated with our ecosystem and appropriate for our projected scale.

## Decision

We have decided to use ASP.NET Core Identity for authentication in our project.

## Rationale

ASP.NET Core Identity was chosen for the following reasons:

1. Native Integration: It's built into the ASP.NET Core framework, ensuring seamless integration with our chosen technology stack.

2. Appropriate Scale: It's well-suited for small to medium-sized applications, aligning with our expectation that we won't need enterprise-scale authentication services like Auth0.

3. Cost-Effective: As part of the ASP.NET Core framework, it doesn't incur additional licensing costs, making it a cost-effective solution for our scale.

4. Customizable: Offers a good balance of out-of-the-box functionality and customization options, allowing us to tailor the authentication process to our needs.

5. Entity Framework Core Integration: Works well with our chosen ORM, allowing for easy management of user data.

6. Security Features: Provides essential security features like password hashing, account lockout, and two-factor authentication out of the box.

7. Extensible: Can be extended to support external authentication providers (like Google or Facebook) if needed in the future.

8. Blazor Support: Designed to work well with Blazor applications, both server-side and WebAssembly.

9. Active Development: Being part of the ASP.NET Core ecosystem, it receives regular updates and security patches.

### Alternatives Considered

1. Auth0:
   - Rejected due to being overly complex and potentially costly for our expected scale.

2. Identity Server:
   - While powerful, it's more suited for scenarios requiring a standalone identity server, which is beyond our current needs.

3. Custom Authentication System:
   - Rejected due to the significant development time required and potential security risks of a custom solution.

4. Azure Active Directory B2C:
   - Considered unnecessary for our scale and would introduce dependency on Azure services.

## Consequences

### Positive

- Simplified development process due to native integration with ASP.NET Core.
- No additional costs for authentication services.
- Consistent user experience across the application.
- Built-in security features reduce the risk of common authentication vulnerabilities.

### Negative

- May require more hands-on management compared to fully managed services like Auth0.
- Could become a limiting factor if the application unexpectedly needs to scale to enterprise level quickly.

### Neutral

- Team members may need to familiarize themselves with ASP.NET Core Identity's specific features and configuration options.
- We'll need to manage user data storage ourselves, typically using our chosen database (PostgreSQL in this case).

## References

- ASP.NET Core Identity documentation: https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity
- Blazor authentication and authorization: https://docs.microsoft.com/en-us/aspnet/core/blazor/security/
- ASP.NET Core Identity with Entity Framework Core: https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity-ef-core