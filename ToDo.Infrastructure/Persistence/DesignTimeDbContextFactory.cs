using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ToDo.Core.Configuration;
using System;

namespace ToDo.Infrastructure
{
  public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
  {
    public ApplicationDbContext CreateDbContext(string[] args)
    {
      var configuration = ApplicationConfig.BuildConfiguration();

      var services = new ServiceCollection();
      services.AddOptions<DatabaseOptions>()
          .Bind(configuration.GetSection(DatabaseOptions.Database))
          .ValidateDataAnnotations();

      var serviceProvider = services.BuildServiceProvider();
      var databaseOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;

      if (string.IsNullOrEmpty(databaseOptions.ConnectionString))
      {
        throw new InvalidOperationException("Connection string not found in DatabaseOptions.");
      }

      var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
      optionsBuilder.UseNpgsql(databaseOptions.ConnectionString);

      return new ApplicationDbContext(optionsBuilder.Options);
    }
  }
}
