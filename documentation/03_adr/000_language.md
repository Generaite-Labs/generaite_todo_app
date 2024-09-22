# ADR: Selection of C# as the Programming Language

## Status

Accepted

## Context

We are developing a To-Do application and need to select a programming language that will best suit our needs, particularly considering our intention to use Large Language Models (LLMs) for code generation.

## Decision

We have decided to use C# version 8 as the primary programming language for this project.

## Rationale

C# was chosen for the following reasons:

1. Strong typing: C# is a strongly-typed language, which provides better compile-time error checking and improved code reliability.

2. Prescriptive nature: C# has a clear and prescriptive syntax, making it easier for developers to write consistent code and for LLMs to generate accurate code.

3. Extensive online presence: There is a large amount of C# code available on the internet, which means LLMs are likely to be well-trained on C# syntax and best practices.

4. Robust library support: C# has a vast ecosystem of libraries and frameworks, providing solutions for various development needs.

5. Excellent templating and code generation support: C# offers powerful tools for templating and code generation, which aligns well with our intention to use LLMs for code generation.

6. Strong backend web UI framework: C# has Blazor, which provides a robust solution for creating web user interfaces.

### Alternatives Considered

1. Python:
   - Rejected due to its dynamic typing, which can lead to runtime errors.
   - Lacks strong templating support.
   - Not as prescriptive as C#.
   - Can be verbose in certain scenarios.
   - Doesn't have a backend web UI framework comparable to Blazor.

2. Rust:
   - Rejected because it's not as prescriptive as C#.
   - Has a complex memory model that could be challenging for LLM code generation.
   - Lacks the level of templating and code generation support found in C#.

3. Elixir:
   - Rejected due to limited library support compared to C#.
   - Has less presence on the internet, potentially leading to less accurate LLM code generation.

## Consequences

### Positive

- Strong typing will lead to more reliable code and easier debugging.
- Prescriptive nature of C# will result in more consistent code across the project.
- Extensive online presence will likely improve the quality of LLM-generated code.
- Robust library support will speed up development and provide solutions for various functionalities.
- Strong templating and code generation support will enhance our ability to work with LLMs.

### Negative

- Developers who are not familiar with C# may need time to adapt.
- The strongly-typed nature of C# may lead to more verbose code in some cases compared to dynamically-typed languages.

### Neutral

- We will need to ensure our development environment is set up to support C# version 8 and its associated tools.

## References

- C# documentation: https://docs.microsoft.com/en-us/dotnet/csharp/
- Blazor documentation: https://docs.microsoft.com/en-us/aspnet/core/blazor/