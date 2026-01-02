using BlogApp.Modules.Identity.Infrastructure;
using BlogApp.Shared.Infrastructure;
using BlogApp.Shared.Presentation.Extensions; // Custom Endpoint Extensions
using BlogApp.Web.Extensions;
using BlogApp.Web.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Reflection;
using System.Text;

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
}); // (Your existing JWT setup)
builder.Services.AddAuthorization();

var app = builder.Build();

// 6. Middleware
app.UseExceptionHandler(); // Uses GlobalExceptionHandler
app.UseAuthentication();
app.UseAuthorization();

// 7. Map Custom Endpoints
app.MapEndpoints();

// 8. Migrations
if (app.Environment.IsDevelopment())
{
    app.ApplyMigrations();
    app.MapScalarApiReference();
}

app.Run();