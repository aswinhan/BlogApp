using BlogApp.Modules.Blog.Presentation;
using BlogApp.Modules.Identity.Presentation;
using BlogApp.ServiceDefaults;
using BlogApp.Shared.Infrastructure.Database; // For Aspire defaults

var builder = WebApplication.CreateBuilder(args);

// 1. Add Service Defaults (Aspire)
builder.AddServiceDefaults();

// 2. Add Shared Services (The Core Engine)
// This registers the InMemorySender, Logging, Validation, Caching, etc.
builder.Services.AddSharedModuleInfrastructure(
    builder.Configuration,
    "WebBootstrapper");

// 3. Add Modules (Vertical Slices)
// Each module registers its own endpoints, services, and db context
builder.Services.AddIdentityModulePresentation(builder.Configuration);
builder.Services.AddBlogModulePresentation(builder.Configuration);

// 4. Add Global Exception Handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Automatic Migration on Startup
if (app.Environment.IsDevelopment())
{
    // Create a scope to resolve scoped services like DbContexts
    using var scope = app.Services.CreateScope();

    // Fetch all registered migrators (Blog, Identity)
    var migrators = scope.ServiceProvider.GetServices<IModuleDatabaseMigrator>();

    foreach (var migrator in migrators)
    {
        await migrator.MigrateAsync(scope);
    }
}

// 5. Configure Pipeline
app.MapDefaultEndpoints(); // Health checks etc.

// 6. Swagger (NSwag)
if (app.Environment.IsDevelopment())
{
    // Uses the extension from Shared.Infrastructure
    app.UseSwaggerDocumentation();
}

app.UseHttpsRedirection();
app.UseExceptionHandler(); // Uses the GlobalExceptionHandler registered above

app.UseAuthentication();
app.UseAuthorization();

// 7. Map Endpoints
app.MapIdentityEndpoints();
app.MapBlogEndpoints();

app.Run();