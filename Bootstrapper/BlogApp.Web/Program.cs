var builder = WebApplication.CreateBuilder(args);

// [OPTIMIZATION 1] Structured Logging (Better than default Console)
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

// 1. Module Definitions
Assembly[] moduleAssemblies = [
    BlogApp.Modules.Identity.Presentation.AssemblyReference.Assembly,
    BlogApp.Modules.Identity.Application.AssemblyReference.Assembly,
    // Add Blog Assemblies here later
];

// 2. Shared Services (Clean Architecture)
// Maintains your Scrutor scanning for MediatR/CQRS
builder.Services.AddSharedInfrastructure(moduleAssemblies);
builder.Services.AddSharedApplication(moduleAssemblies);

// 3. Module Services
// [OPTIMIZATION 2] Encapsulation
// We moved the JWT Logic INSIDE this method (See Step 3 below). 
// Program.cs shouldn't know about "SecretKey" or "TokenValidation".
builder.Services.AddIdentityInfrastructure(builder.Configuration);

// 4. Custom Endpoints
builder.Services.AddEndpoints(moduleAssemblies);

// 5. API & Open API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi("v1");
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// 6. Advanced Rate Limiting (Anti-DDoS)
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Strict Policy for Auth (Login/Register) - 5 attempts per min
    options.AddFixedWindowLimiter("AuthPolicy", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 0;
    });

    // Global Policy - Token Bucket is better for smooth traffic
    options.AddTokenBucketLimiter("GlobalPolicy", opt =>
    {
        opt.TokenLimit = 100;
        opt.TokensPerPeriod = 10;
        opt.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
        opt.QueueLimit = 5;
    });
});

// 7. High-Performance JSON
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();

// [OPTIMIZATION 3] Professional Security Headers (Replaces your manual app.Use)
// This uses the NuGet 'NetEscapades.AspNetCore.SecurityHeaders'
// It automatically handles edge cases for CSP, HSTS, and X-Content-Type.
var securityPolicy = new HeaderPolicyCollection()
    .AddDefaultSecurityHeaders()
    .AddContentSecurityPolicy(builder =>
    {
        builder.AddDefaultSrc().Self();
        builder.AddScriptSrc().Self().UnsafeInline(); // Needed for Scalar/Swagger sometimes
        builder.AddFrameAncestors().None(); // Anti-Clickjacking
    })
    .AddPermissionsPolicy(builder =>
    {
        builder.AddAccelerometer().None();
        builder.AddCamera().None();
        builder.AddGeolocation().None();
    });

app.UseSecurityHeaders(securityPolicy);

// 8. Pipeline Order (Critical for Security)
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.ApplyMigrations();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection(); // Force SSL
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

// 9. Map Endpoints
app.MapEndpoints();

app.Run();