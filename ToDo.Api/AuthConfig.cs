using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using ToDo.Domain.Entities;
using ToDo.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace ToDo.Api;

public static class AuthConfig
{
    public static void ConfigureAuthenticationAndIdentity(WebApplicationBuilder builder)
    {
        builder.Services.TryAddSingleton(TimeProvider.System);
        builder.Services.AddDataProtection();

        // Establish cookie authentication
        builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddIdentityCookies();

        // Configure Identity
        builder.Services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<TodoDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders()
        .AddApiEndpoints();

        // Configure application cookie
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            options.SlidingExpiration = true;
            options.LoginPath = "/login";
            
            // Apply SameSite settings only in development
            if (builder.Environment.IsDevelopment())
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            }
            else
            {
                // For production, use more secure defaults
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            }
        });

        // Configure authorization
        builder.Services.AddAuthorizationBuilder();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowBlazorOrigin",
                corsBuilder => corsBuilder
                    .WithOrigins(builder.Configuration["Application:FrontendUrl"] ?? throw new InvalidOperationException("FrontendUrl is not configured"))
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
        });

        // Configure Swagger
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "ToDo API", Version = "v1" });
            c.AddSecurityDefinition("cookieAuth", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Cookie,
                Name = ".AspNetCore.Identity.Application"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "cookieAuth" }
                    },
                    new string[] {}
                }
            });
        });
    }

    public static void ConfigureAuth(WebApplication app)
    {
        // Activate the CORS policy
        app.UseCors("AllowBlazorOrigin");

        // Enable authentication and authorization
        app.UseAuthentication();
        app.UseAuthorization();

        // Map Identity API endpoints
        app.MapIdentityApi<ApplicationUser>();

        // Provide an endpoint to clear the cookie for logout
        app.MapPost("/logout", async (SignInManager<ApplicationUser> signInManager, [FromBody] object empty) =>
        {
            if (empty is not null)
            {
                await signInManager.SignOutAsync();
                return Results.Ok();
            }
            return Results.Unauthorized();
        }).RequireAuthorization();

        // Provide an endpoint for user roles
        app.MapGet("/roles", (System.Security.Claims.ClaimsPrincipal user) =>
        {
            if (user.Identity is not null && user.Identity.IsAuthenticated)
            {
                var identity = (System.Security.Claims.ClaimsIdentity)user.Identity;
                var roles = identity.FindAll(identity.RoleClaimType)
                    .Select(c => new
                    {
                        c.Issuer,
                        c.OriginalIssuer,
                        c.Type,
                        c.Value,
                        c.ValueType
                    });

                return TypedResults.Json(roles);
            }

            return Results.Unauthorized();
        }).RequireAuthorization();
    }
}
