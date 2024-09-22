# Create the solution and project structure:
   - Open a terminal and navigate to your desired project directory
   - Create a new solution:
     ```
     dotnet new sln -n ToDo
     ```
   - Create the main projects:
     ```
     dotnet new webapi -n ToDo.Api
     dotnet new blazorwasm -n ToDo.WebClient
     dotnet new classlib -n ToDo.Domain
     dotnet new classlib -n ToDo.Application
     dotnet new classlib -n ToDo.Infrastructure
     ```
   - Add projects to the solution:
     ```
     dotnet sln add ToDo.Api/ToDo.Api.csproj
     dotnet sln add ToDo.WebClient/ToDo.WebClient.csproj
     dotnet sln add ToDo.Domain/ToDo.Domain.csproj
     dotnet sln add ToDo.Application/ToDo.Application.csproj
     dotnet sln add ToDo.Infrastructure/ToDo.Infrastructure.csproj
     ```

# Set up project dependencies:
  - Add project references:
    ```
    dotnet add ToDo.Api reference ToDo.Application ToDo.Infrastructure
    dotnet add ToDo.Application reference ToDo.Domain
    dotnet add ToDo.Infrastructure reference ToDo.Domain
    ```

# Install necessary NuGet packages:
   - For ToDo.Api:
     ```
     dotnet add ToDo.Api package Microsoft.EntityFrameworkCore.Design
     dotnet add ToDo.Api package Serilog.AspNetCore
     dotnet add ToDo.Api package Serilog.Sinks.Console
     ```
   - For ToDo.Infrastructure:
     ```
     dotnet add ToDo.Infrastructure package Microsoft.EntityFrameworkCore
     dotnet add ToDo.Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL
     ```
   - For ToDo.Application:
     ```
     dotnet add ToDo.Application package FluentValidation
     dotnet add ToDo.Application package FluentValidation.DependencyInjectionExtensions
     ```

# Configure the database connection:
   - In ToDo.Api, update appsettings.json:
     ```json
     {
       "ConnectionStrings": {
         "DefaultConnection": "__TODO_DB_CONNECTION__"
       }
     }
     ```
   - Set up a user secret for development:
     ```
     dotnet user-secrets init --project ToDo.Api
     dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=ToDo;Username=postgres;Password=postgres" --project ToDo.Api
     ```

# Set up Entity Framework Core:
   - In ToDo.Infrastructure, create DatabaseConfig.cs:
     ```csharp
      namespace ToDo.Infrastructure;

      public class DatabaseConfig
      {
        public required string ConnectionString { get; set; }
      }
     ```
   - Create TodoDbContext.cs in ToDo.Infrastructure:
     ```csharp
      using Microsoft.EntityFrameworkCore;

      namespace ToDo.Infrastructure;

      public class TodoDbContext : DbContext
      {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          base.OnModelCreating(modelBuilder);
          // Add any additional model configurations here
        }

        // Add DbSet properties for your entities here as you create them
        // For example:
        // public DbSet<TodoItem> TodoItems { get; set; }
      }
     ```
   - Create InfrastructureModule.cs in ToDo.Infrastructure:
     ```csharp
     public static class InfrastructureModule
     {
         public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
         {
             services.Configure<DatabaseConfig>(options => 
                 options.ConnectionString = configuration.GetConnectionString("DefaultConnection"));
             
             services.AddDbContext<TodoDbContext>((serviceProvider, options) =>
             {
                 var dbConfig = serviceProvider.GetRequiredService<IOptions<DatabaseConfig>>();
                 options.UseNpgsql(dbConfig.Value.ConnectionString);
             });

             services.AddIdentity<ApplicationUser, IdentityRole>()
                 .AddEntityFrameworkStores<TodoDbContext>()
                 .AddDefaultTokenProviders();

             // Register other infrastructure services...

             return services;
         }
     }
     ```
  - Add Infrastructure Services to your API Program.cs:
    ```csharp
    using ToDo.Infrastructure;
    ...
    builder.Services.AddInfrastructureServices(builder.Configuration);
    ```

* Configure Serilog for logging:
   - Update ToDo.Api/Program.cs:
     ```csharp
     using Serilog;

     Log.Logger = new LoggerConfiguration()
         .WriteTo.Console()
         .CreateLogger();

     builder.Host.UseSerilog();
     ```

* Set up ASP.NET Core Identity:
  - Install identity in Domain and Infrastructure:
    ```bash
    dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
    ```
   - Create ApplicationUser.cs in ToDo.Domain:
     ```csharp
     public class ApplicationUser : IdentityUser
     {
         // Add custom user properties here
     }
     ```

* Use core Identity:
  - Add Identity to Infrastructure:
  ```bash
  dotnet add ToDo.Infrastructure package Microsoft.AspNetCore.Identity
  ```
  - Add Identity and configuration to InfrastructureModule.cs:
  ```csharp
    using Microsoft.AspNetCore.Identity;
    using ToDo.Domain.Entities;
    ...
    services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<TodoDbContext>()
        .AddDefaultTokenProviders();

    services.Configure<IdentityOptions>(options =>
    {
      // Password settings
      options.Password.RequireDigit = true;
      options.Password.RequireLowercase = true;
      options.Password.RequireNonAlphanumeric = true;
      options.Password.RequireUppercase = true;
      options.Password.RequiredLength = 6;
      options.Password.RequiredUniqueChars = 1;

      // Lockout settings
      options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
      options.Lockout.MaxFailedAccessAttempts = 5;
      options.Lockout.AllowedForNewUsers = true;

      // User settings
      options.User.AllowedUserNameCharacters =
                  "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
      options.User.RequireUniqueEmail = false;
    });
    ```
  - Make ToDoDbContext depend on IdentityDbContext
    ```csharp
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using ToDo.Domain.Entities;

    namespace ToDo.Infrastructure;

    public class TodoDbContext : IdentityDbContext<ApplicationUser>
    {
      public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }
    }
    ```

#  Set up the initial API controller:
  - Add these lines to your ToDo.Api Program.cs:
    ```csharp
    ...
    builder.Services.AddControllers();
    ...
    app.MapControllers();
    ```
  - Create ApiController.cs in ToDo.Api/Controllers:
    ```csharp
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiController : ControllerBase
    {
    }
    ```

*  Configure CORS:
    - In ToDo.Api/Program.cs:
      ```csharp
      builder.Services.AddCors(options =>
      {
          options.AddPolicy("AllowBlazorOrigin",
              builder => builder.WithOrigins("https://localhost:5001")
                                .AllowAnyMethod()
                                .AllowAnyHeader());
      });

      // ... in the Configure method:
      app.UseCors("AllowBlazorOrigin");
      ```

*  Configure SignalR:
    - Add SignalR client-side library to ToDo.Client:
      ```
      dotnet add ToDo.WebClient package Microsoft.AspNetCore.SignalR.Client
      ```
    - In ToDo.Api/Program.cs:
      ```csharp
      builder.Services.AddSignalR();
      ```

* Set up the test project:
    ```
    dotnet new xunit -n ToDo.Tests
    dotnet sln add ToDo.Tests/ToDo.Tests.csproj
    dotnet add ToDo.Tests package bUnit
    dotnet add ToDo.Tests package AutoFixture
    dotnet add ToDo.Tests package FluentAssertions
    ```

