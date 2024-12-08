using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace ToDo.Core.Configuration
{
  public static class ApplicationConfig
  {
    public static IConfiguration BuildConfiguration()
    {
      var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
      var assembly = Assembly.GetExecutingAssembly();
      var assemblyPath = Path.GetDirectoryName(assembly.Location);

      // If assemblyPath is null, fall back to the current directory
      var basePath = assemblyPath ?? AppDomain.CurrentDomain.BaseDirectory;

      var configPath = Path.Combine(basePath, "appsettings.json");
      if (!File.Exists(configPath))
      {
        throw new FileNotFoundException($"Configuration file not found: {configPath}");
      }

      return new ConfigurationBuilder()
          .SetBasePath(basePath)
          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
          .AddJsonFile($"appsettings.{environment}.json", optional: true)
          .AddEnvironmentVariables()
          .AddEnvironmentVariables("TODO_")  // This line allows prefixing env vars with TODO_
          .Build();
    }

    public static void AddApplicationConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
      services.AddOptions<DatabaseOptions>()
          .Bind(configuration.GetSection(DatabaseOptions.Database))
          .ValidateDataAnnotations()
          .ValidateOnStart();

      services.AddOptions<ApplicationSettings>()
          .Bind(configuration.GetSection(ApplicationSettings.Application))
          .ValidateDataAnnotations()
          .ValidateOnStart();

      services.AddOptions<JwtOptions>()
          .Bind(configuration.GetSection(JwtOptions.Jwt))
          .ValidateDataAnnotations()
          .ValidateOnStart();

      // Register IOptions<T> for dependency injection
      services.AddSingleton(resolver =>
          resolver.GetRequiredService<IOptions<DatabaseOptions>>().Value);
      services.AddSingleton(resolver =>
          resolver.GetRequiredService<IOptions<ApplicationSettings>>().Value);
      services.AddSingleton(resolver =>
          resolver.GetRequiredService<IOptions<JwtOptions>>().Value);
    }
  }
}
