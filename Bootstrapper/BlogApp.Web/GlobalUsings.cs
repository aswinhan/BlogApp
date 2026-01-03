global using BlogApp.Modules.Identity.Infrastructure;
global using BlogApp.Shared.Application;    // Keep your existing Application logic
global using BlogApp.Shared.Infrastructure; // Keep your existing Shared logic
global using BlogApp.Shared.Presentation.Extensions;
global using BlogApp.Web.Extensions;        // Ensure this namespace exists for migration extensions
global using BlogApp.Web.Middleware;
global using BlogApp.Web.Serialization;     // Keep your JSON Context
global using Microsoft.AspNetCore.RateLimiting;
global using Scalar.AspNetCore;
global using Serilog;
global using System.Reflection;
global using BlogApp.Modules.Identity.Infrastructure.Database;
global using Microsoft.EntityFrameworkCore;
global using Npgsql;
global using Microsoft.AspNetCore.Diagnostics;
global using Microsoft.AspNetCore.Mvc;
global using System.Threading.RateLimiting;