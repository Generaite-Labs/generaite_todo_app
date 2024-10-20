using ToDo.Infrastructure;
using ToDo.Core.Configuration;
using Serilog;
using ToDo.Api;
using Serilog.Events;
using ToDo.Application.Mappers;
using Microsoft.AspNetCore.Identity;
using ToDo.Domain.Interfaces;
using ToDo.Application.Services;
using ToDo.Application.EventHandlers;
using ToDo.Domain.Events;
using ToDo.Infrastructure.Services;
using ToDo.Domain.Entities;
using ToDo.Infrastructure.Configuration;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateBootstrapLogger();

try
{
    // Configure Serilog
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"));

    // Add configuration
    builder.Configuration.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"), optional: false, reloadOnChange: true)
                         .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"appsettings.{builder.Environment.EnvironmentName}.json"), optional: true)
                         .AddEnvironmentVariables("TODO_");

    // Configure ApplicationSettings
    var applicationSettings = new ApplicationSettings();
    builder.Configuration.GetSection(ApplicationSettings.Application).Bind(applicationSettings);
    builder.Services.Configure<ApplicationSettings>(
        builder.Configuration.GetSection(ApplicationSettings.Application));


    AuthConfig.ConfigureAuthenticationAndIdentity(builder);

    // Debug: Print ApplicationSettings
    Console.WriteLine($"ApplicationSettings.FrontendUrl: {applicationSettings.FrontendUrl}");
    Console.WriteLine($"ApplicationSettings.ApiBaseUrl: {applicationSettings.ApiBaseUrl}");

    // Add infrastructure services
    builder.Services.AddInfrastructureServices(builder.Configuration);

    builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

    // Add this line to register the EmailSender
    if (builder.Environment.IsDevelopment())
    {
        builder.Services.AddTransient<IEmailSender, DevelopmentEmailSender>();
        builder.Services.AddTransient<IEmailSender<ApplicationUser>, DevelopmentEmailSender>();
    }
    else
    {
        builder.Services.AddTransient<IEmailSender, EmailSender>();
        builder.Services.AddTransient<IEmailSender<ApplicationUser>, EmailSender>();
    }

    builder.Services.AddControllers();
    builder.Services.AddSignalR();

    builder.Services.AddAutoMapper(typeof(TodoItemMappingProfile));

    // Register domain event handler
    builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
    builder.Services.AddScoped<IDomainEventHandler<TodoItemCompletedEvent>, TodoItemCompletedEventHandler>();

    builder.Services.AddScoped<IDomainEventService, DomainEventService>();

    builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("EmailConfiguration"));

    builder.Services.AddEndpointsApiExplorer();
    
    // Build the application
    var app = builder.Build();

    // Add global exception handling middleware
    app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

    // Add Serilog request logging
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "[{Timestamp:HH:mm:ss}] HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    });

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API v1"));
    }

    // Configure authentication and authorization
    AuthConfig.ConfigureAuth(app);

    app.MapControllers();

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
