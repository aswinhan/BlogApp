using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// -----------------------------------------------------------------------
// 1. POSTGRESQL (Raw Container)
// -----------------------------------------------------------------------
// We use AddContainer to avoid Aspire's managed resource conflicts.
var postgres = builder.AddContainer("blog-postgres", "postgres", "latest")
    .WithEnvironment("POSTGRES_PASSWORD", "postgres")
    .WithEnvironment("POSTGRES_USER", "postgres")
    .WithEnvironment("POSTGRES_DB", "blogapp")
    // [CRITICAL] Use a Named Volume with a unique name.
    // This avoids "directory not empty" errors on Windows.
    .WithVolume("blog-pg-data-v6", "/var/lib/postgresql/data")
    // Map Host 5445 -> Container 5432
    // We name the endpoint "postgres-custom" to avoid 'tcp' collisions.
    .WithEndpoint(port: 5445, targetPort: 5432, name: "postgres-custom");

// -----------------------------------------------------------------------
// 2. PGADMIN (UI)
// -----------------------------------------------------------------------
var pgadmin = builder.AddContainer("pgadmin", "dpage/pgadmin4", "latest")
    .WithEnvironment("PGADMIN_DEFAULT_EMAIL", "admin@admin.com")
    .WithEnvironment("PGADMIN_DEFAULT_PASSWORD", "admin")
    .WithEndpoint(port: 5050, targetPort: 80, name: "pgadmin-http");

// -----------------------------------------------------------------------
// 3. REDIS (Raw Container)
// -----------------------------------------------------------------------
var redis = builder.AddContainer("blog-redis", "redis", "latest")
    .WithVolume("blog-redis-data-v6", "/data")
    // Map Host 6000 -> Container 6379
    .WithEndpoint(port: 6000, targetPort: 6379, name: "redis-custom");

// -----------------------------------------------------------------------
// 4. WEB API
// -----------------------------------------------------------------------
builder.AddProject<Projects.BlogApp_Web>("blog-api")
    .WaitFor(postgres)
    .WaitFor(redis)
    // FORCE connection strings. Use 'host.docker.internal' for reliability.
    .WithEnvironment("ConnectionStrings__postgres", "Host=host.docker.internal;Port=5445;Database=blogapp;Username=postgres;Password=postgres")
    .WithEnvironment("ConnectionStrings__redis", "host.docker.internal:6000")
    .WithHttpsEndpoint(port: 7000, name: "api-https")
    .WithHttpEndpoint(port: 7001, name: "api-http");

builder.Build().Run();