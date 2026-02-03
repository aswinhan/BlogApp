using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// 1. WEB API (Running directly on Host)
// We remove .WithReference(postgres/redis) because they are no longer Aspire resources.
// The Web API will look at its own appsettings.json for connections.
builder.AddProject<Projects.BlogApp_Web>("blog-api")
    .WithHttpsEndpoint(port: 7000, name: "api-https")
    .WithHttpEndpoint(port: 7001, name: "api-http");

builder.Build().Run();