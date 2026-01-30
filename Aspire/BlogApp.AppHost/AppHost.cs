using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// 1. Create a Parameter for the Password
// This creates a safe resource that Aspire can manage
var passwordParameter = builder.AddParameter("postgres-password", "postgres");

// 2. Postgres (Port 5445)
var postgres = builder.AddPostgres("blog-postgres", password: passwordParameter) // [FIX] Pass parameter here
                      .WithImage("postgres", "latest")
                      .WithDataVolume()
                      .WithPgAdmin(pg =>
                      {
                          pg.WithImage("dpage/pgadmin4", "latest")
                            .WithEndpoint(port: 5050, targetPort: 80, name: "pgadmin-ui");
                      })
                      .WithEndpoint(port: 5445, targetPort: 5432, name: "primary");

// 3. Redis (Port 6000)
var redis = builder.AddRedis("blog-redis")
                   .WithImage("redis", "latest")
                   .WithDataVolume()
                   .WithEndpoint(port: 6000, targetPort: 6379, name: "primary");

// 4. API Project (Port 7000)
builder.AddProject<Projects.BlogApp_Web>("blog-api")
                 .WithReference(postgres)
                 .WithReference(redis)
                 .WaitFor(postgres)
                 .WaitFor(redis)
                 .WithHttpsEndpoint(port: 7000, name: "api-https")
                 .WithHttpEndpoint(port: 7001, name: "api-http");

builder.Build().Run();