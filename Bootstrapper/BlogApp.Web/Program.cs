
var builder = WebApplication.CreateBuilder(args);

// [OPTIMIZATION 1] Structured Logging
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

// 1. Module Discovery
Assembly[] moduleAssemblies = [
    BlogApp.Modules.Identity.Presentation.AssemblyReference.Assembly,
    BlogApp.Modules.Identity.Application.AssemblyReference.Assembly,
    BlogApp.Modules.Blog.Application.AssemblyReference.Assembly
];

// 2. Shared Services
builder.Services.AddSharedInfrastructure(moduleAssemblies, builder.Configuration);
builder.Services.AddSharedApplication(moduleAssemblies);

// 3. Module Services
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddBlogInfrastructure(builder.Configuration);

// 4. Global API Handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// [OPTIMIZATION 2] Advanced Rate Limiting (Anti-DDoS)
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("fixed", policy =>
    {
        policy.PermitLimit = 100; // 100 requests
        policy.Window = TimeSpan.FromMinutes(1); // per minute
        policy.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        policy.QueueLimit = 5;
    });
});

// [OPTIMIZATION 3] High-Performance JSON
builder.Services.ConfigureHttpJsonOptions(options =>
{
    // Serialize Enums as Strings (Readable)
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    // Ignore null values to save bandwidth
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

var app = builder.Build();

// --- HTTP Request Pipeline ---

// 1. Security Headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});

// 2. Rate Limiting (Must be early)
app.UseRateLimiter();

// 3. Exception Handling
app.UseExceptionHandler();

// 4. Swagger (Development Only)
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();

    // Auto-Migrate Databases
    using var scope = app.Services.CreateScope();
    await scope.MigrateModuleDatabasesAsync();
}

app.UseHttpsRedirection();

// 5. Auth
app.UseAuthentication();
app.UseAuthorization();

// 6. Map All Endpoints
app.MapApiEndpoints();

app.Run();