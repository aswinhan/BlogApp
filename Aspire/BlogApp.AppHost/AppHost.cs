using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// 1. Password Parameter
// Managed secret with default value "postgres"
var dbPassword = builder.AddParameter("postgres-password", "postgres");

// 2. Postgres
var postgres = builder.AddPostgres("blog-postgres", password: dbPassword)
                      .WithImage("postgres", "latest")
                      .WithDataVolume()
                      .WithPgAdmin(pg =>
                      {
                          pg.WithImage("dpage/pgadmin4", "latest")
                            // Use unique name to avoid conflict with default http endpoint
                            .WithEndpoint(port: 5050, targetPort: 80, name: "pgadmin-custom");
                      })
                      // Map Host 5445 -> Container 5432. 
                      // [FIX] Use unique name "postgres-tcp" to avoid conflict with default 'tcp' endpoint
                      .WithEndpoint(port: 5445, targetPort: 5432, name: "postgres-tcp");

// 3. Redis
var redis = builder.AddRedis("blog-redis")
                   .WithImage("redis", "latest")
                   .WithDataVolume()
                   // Map Host 6000 -> Container 6379
                   // [FIX] Use unique name "redis-tcp" to avoid conflict
                   .WithEndpoint(port: 6000, targetPort: 6379, name: "redis-tcp");

// 4. API Project
builder.AddProject<Projects.BlogApp_Web>("blog-api")
                 .WithReference(postgres)
                 .WithReference(redis)
                 .WaitFor(postgres)
                 .WaitFor(redis)
                 .WithHttpsEndpoint(port: 7000, name: "api-https")
                 .WithHttpEndpoint(port: 7001, name: "api-http");

builder.Build().Run();