var builder = DistributedApplication.CreateBuilder(args);

// 1. Define Postgres Container
var postgres = builder.AddPostgres("postgres")
                      .WithDataVolume() // Persist data across restarts
                      .WithPgAdmin();   // Optional: Adds a UI to manage DB

// 2. Define Redis Container
var redis = builder.AddRedis("redis")
                   .WithDataVolume();

// 3. Define the API Project
// Note: "Projects.BlogApp_Web" is generated from your project name "BlogApp.Web"
builder.AddProject<Projects.BlogApp_Web>("blog-api")
                 .WithReference(postgres) // Inject "ConnectionStrings:postgres"
                 .WithReference(redis)    // Inject "ConnectionStrings:redis"
                 .WaitFor(postgres)       // Wait for DB to be ready before starting API
                 .WaitFor(redis);

builder.Build().Run();