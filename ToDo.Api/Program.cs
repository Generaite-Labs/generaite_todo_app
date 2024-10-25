using ToDo.Infrastructure;
using ToDo.Core.Configuration;
using Serilog;
using ToDo.Api.Configuration;
using ToDo.Application.Mappers;
using Microsoft.AspNetCore.Identity;
using ToDo.Domain.Interfaces;
using ToDo.Application.Services;
using ToDo.Domain.Events;
using ToDo.Infrastructure.Services;
using ToDo.Domain.Entities;
using ToDo.Infrastructure.Configuration;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

try
{
    LoggingConfig.ConfigureLogging(builder);

    builder.Configuration.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"), optional: false, reloadOnChange: true)
                         .AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"appsettings.{builder.Environment.EnvironmentName}.json"), optional: true)
                         .AddEnvironmentVariables("TODO_");

    var applicationSettings = new ApplicationSettings();
    builder.Configuration.GetSection(ApplicationSettings.Application).Bind(applicationSettings);
    builder.Services.Configure<ApplicationSettings>(
        builder.Configuration.GetSection(ApplicationSettings.Application));

    AuthConfig.ConfigureAuthenticationAndIdentity(builder);
    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

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
    builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
    builder.Services.AddScoped<IDomainEventService, DomainEventService>();
    builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("EmailConfiguration"));
    builder.Services.AddEndpointsApiExplorer();

    var app = builder.Build();

    LoggingConfig.ConfigureRequestLogging(app);

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API v1"));
    }

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
