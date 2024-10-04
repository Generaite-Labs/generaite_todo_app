using ToDo.Infrastructure;
using ToDo.Core.Configuration;
using Serilog;
using ToDo.Api;
using Serilog.Events;
using ToDo.Application.Mappers;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
  var builder = WebApplication.CreateBuilder(args);

  // Configure Serilog
  builder.Host.UseSerilog((context, services, configuration) => configuration
      .ReadFrom.Configuration(context.Configuration)
      .ReadFrom.Services(services)
      .Enrich.FromLogContext()
      .WriteTo.Console(new Serilog.Formatting.Compact.CompactJsonFormatter()));

  // Add configuration
  builder.Configuration.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"), optional: false, reloadOnChange: true)
                       .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"appsettings.{builder.Environment.EnvironmentName}.json"), optional: true)
                       .AddEnvironmentVariables("TODO_");

  // Add services to the container.
  builder.Services.AddEndpointsApiExplorer();
  builder.Services.AddSwaggerGen();

  // Add application configuration
  builder.Services.AddOptions<DatabaseOptions>()
      .Bind(builder.Configuration.GetSection(DatabaseOptions.Database))
      .ValidateDataAnnotations()
      .ValidateOnStart();

  // Add infrastructure services
  builder.Services.AddInfrastructureServices(builder.Configuration);

  builder.Services.AddControllers();
  builder.Services.AddSignalR();

  builder.Services.AddCors(options =>
  {
    options.AddPolicy("AllowBlazorOrigin",
                builder => builder.WithOrigins("https://localhost:5146")
                                  .AllowAnyMethod()
                                  .AllowAnyHeader());
  });

  builder.Services.AddAutoMapper(typeof(TodoItemMappingProfile));

  var app = builder.Build();

  // Add global exception handling middleware
  app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

  // Add Serilog request logging
  app.UseSerilogRequestLogging(options =>
  {
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
  });

  // Configure the HTTP request pipeline.
  if (app.Environment.IsDevelopment())
  {
    app.UseSwagger();
    app.UseSwaggerUI();
  }

  app.UseHttpsRedirection();
  app.UseAuthentication();
  app.UseAuthorization();
  app.MapControllers();
  app.UseCors("AllowBlazorOrigin");

  var summaries = new[]
  {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

  app.MapGet("/weatherforecast", () =>
  {
    var forecast = Enumerable.Range(1, 5).Select(index =>
          new WeatherForecast
          (
              DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
              Random.Shared.Next(-20, 55),
              summaries[Random.Shared.Next(summaries.Length)]
          ))
          .ToArray();
    return forecast;
  })
  .WithName("GetWeatherForecast")
  .WithOpenApi();

  app.Run();
}
catch (Exception ex)
{
  Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
  Log.CloseAndFlush();
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
  public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}