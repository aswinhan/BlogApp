using BlogApp.Modules.Identity.Infrastructure;
using BlogApp.Shared.Infrastructure;
using BlogApp.Shared.Presentation.Extensions; // Custom Endpoint Extensions
using BlogApp.Web.Extensions;
using BlogApp.Web.Middleware;
using BlogApp.Web.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Reflection;
using System.Text;
using BlogApp.Shared.Application;

var builder = WebApplication.CreateBuilder(args);

// 1. Definitions
Assembly[] moduleAssemblies = [
    BlogApp.Modules.Identity.Presentation.AssemblyReference.Assembly,
    BlogApp.Modules.Identity.Application.AssemblyReference.Assembly,
    // Add Blog Assemblies later
];

// 2. Shared Services (Dispatcher, Pipeline, Validations)
// Note: We pass assemblies here so Scrutor can scan for Handlers
builder.Services.AddSharedInfrastructure(moduleAssemblies);
builder.Services.AddSharedApplication(moduleAssemblies);

// 3. Module Services
builder.Services.AddIdentityInfrastructure(builder.Configuration);
// builder.Services.AddBlogInfrastructure(builder.Configuration);

// 4. Custom Endpoints (Scanning)
builder.Services.AddEndpoints(moduleAssemblies);

// 5. API & Auth
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi("v1");
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    var configuration = builder.Configuration;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidAudience = configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]!))
    };
}); 
builder.Services.AddAuthorization();

// Add Rate Limiting to prevent brute force
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Policy: "AuthPolicy" - Strict limits for Login/Register
    options.AddFixedWindowLimiter("AuthPolicy", opt =>
    {
        opt.PermitLimit = 5; // Max 5 attempts
        opt.Window = TimeSpan.FromMinutes(1); // Per 1 minute
        opt.QueueLimit = 0; // No queueing, reject immediately
    });

    // Policy: "GlobalPolicy" - Generous limits for normal API usage
    options.AddTokenBucketLimiter("GlobalPolicy", opt =>
    {
        opt.TokenLimit = 100;
        opt.QueueLimit = 5;
        opt.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
        opt.TokensPerPeriod = 10;
    });
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    // Use the generated context for maximum speed
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    // Prevent site from being embedded in iframes (Clickjacking)
    context.Response.Headers.Append("X-Frame-Options", "DENY");

    // Prevent browser from sniffing MIME types
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

    // Force HTTPS
    context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");

    // Basic CSP (Content Security Policy) - Limit where scripts can run from
    context.Response.Headers.Append("Content-Security-Policy", "default-src 'self';");

    await next();
}); 

// 6. Middleware
app.UseExceptionHandler(); // Uses GlobalExceptionHandler
app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

// 7. Map Custom Endpoints
app.MapEndpoints();

// 8. Migrations
if (app.Environment.IsDevelopment())
{
    app.ApplyMigrations();
    app.MapScalarApiReference();
}

app.Run();