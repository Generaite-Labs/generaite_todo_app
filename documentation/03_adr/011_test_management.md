# ADR: Testing Strategy for To-Do Application

## Status
Accepted

## Context
Effective testing is crucial for ensuring the reliability, maintainability, and quality of our To-Do application. We need a comprehensive testing strategy that covers unit testing, integration testing, and end-to-end testing, while also facilitating a Test-Driven Development (TDD) approach.

## Decision
We will implement the following testing strategy:

1. Use xUnit as our primary testing framework
2. Employ bUnit for testing Blazor components
3. Organize tests in a single test project with subdirectories for each application project
4. Write unit tests for all components and services
5. Implement minimal integration tests, preferring bUnit over Selenium or Playwright
6. Create DB mocks for all repositories to enable code-only unit tests
7. Use AutoFixture for test data generation
8. Adopt SpecFlow for Behavior-Driven Development (BDD) and end-to-end testing
9. Use DB mocks for unit tests and an actual database for integration/end-to-end tests
10. Follow a TDD approach throughout development

## Rationale
- xUnit provides a modern, extensible testing framework for .NET applications
- bUnit is specifically designed for testing Blazor components, offering powerful capabilities for component testing
- A single test project with subdirectories maintains organization while keeping all tests in one place
- Comprehensive unit testing ensures individual components work as expected
- Minimal integration tests with bUnit allow us to test component interactions without the complexity of full browser automation
- DB mocks for repositories enable fast, repeatable unit tests without database dependencies
- AutoFixture simplifies test data creation and reduces test arrangement code
- SpecFlow enables BDD, improving communication between technical and non-technical team members
- Using actual databases for integration/end-to-end tests ensures we catch issues related to database interactions
- TDD promotes good design, maintainable code, and high test coverage

## Implementation Details
1. Create a single test project named `ToDo.Tests` with subdirectories mirroring the main project structure
2. Add NuGet packages: xUnit, bUnit, AutoFixture, SpecFlow, and necessary DB mocking libraries
3. Implement repository interfaces and DB mock classes for unit testing
4. Write xUnit test classes for all non-Blazor components and services
5. Create bUnit test classes for Blazor components
6. Develop SpecFlow feature files and step definitions for end-to-end scenarios
7. Set up a test database for integration and end-to-end tests
8. Implement continuous integration to run all tests on each commit

## Consequences
### Positive
- Comprehensive test coverage across all layers of the application
- Fast and reliable unit tests due to DB mocking
- Improved component testing with bUnit
- Enhanced communication and understanding of requirements through SpecFlow
- Facilitated TDD approach leading to better design and maintainability
- Simplified test data generation with AutoFixture

### Negative
- Initial setup time for the testing infrastructure
- Learning curve for team members unfamiliar with bUnit or SpecFlow
- Potential for slower end-to-end tests due to actual database usage
- Maintenance overhead of keeping DB mocks in sync with actual repository implementations

## Future Considerations
- Evaluate the need for additional integration testing tools as the application grows
- Monitor test execution times and optimize slow-running tests
- Consider implementing property-based testing for more thorough test coverage
- Explore options for visual regression testing of Blazor components

## Implementation Example
Here's a basic example of how we might structure our test project and write tests:

```csharp
// ToDo.Tests/Domain/TaskTests.cs
public class TaskTests
{
    [Fact]
    public void Task_WhenCreated_ShouldHaveCorrectProperties()
    {
        var fixture = new Fixture();
        var task = fixture.Create<Task>();

        Assert.NotNull(task);
        Assert.NotEmpty(task.Title);
        Assert.NotNull(task.CreatedAt);
    }
}

// ToDo.Tests/Blazor/Components/TaskListTests.cs
public class TaskListTests : TestContext
{
    [Fact]
    public void TaskList_WhenRendered_ShouldDisplayTasks()
    {
        var tasks = new Fixture().CreateMany<Task>(3).ToList();
        var cut = RenderComponent<TaskList>(parameters => parameters
            .Add(p => p.Tasks, tasks));

        cut.FindAll(".task-item").Count.Should().Be(3);
    }
}

// ToDo.Tests/Features/CreateTask.feature
Feature: Create Task
  As a user
  I want to create a new task
  So that I can keep track of my to-dos

  Scenario: Create a valid task
    Given I am on the create task page
    When I enter "Buy groceries" as the task title
    And I enter "Milk, bread, eggs" as the task description
    And I click the create button
    Then I should see "Task created successfully" message
    And the task "Buy groceries" should appear in my task list

// ToDo.Tests/Features/CreateTaskSteps.cs
[Binding]
public class CreateTaskSteps
{
    private readonly ITaskRepository _taskRepository;
    // Other necessary setup...

    [Given("I am on the create task page")]
    public void GivenIAmOnTheCreateTaskPage()
    {
        // Navigation logic...
    }

    [When("I enter {string} as the task title")]
    public void WhenIEnterTheTaskTitle(string title)
    {
        // Input logic...
    }

    // Other step definitions...
}
```