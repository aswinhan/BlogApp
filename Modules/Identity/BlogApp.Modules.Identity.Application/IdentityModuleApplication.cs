namespace BlogApp.Modules.Identity.Application;

public static class IdentityModuleApplication
{
    public static IServiceCollection AddIdentityModuleApplication(this IServiceCollection services, IConfiguration _)
    {
        // Currently, we don't have specific services to register here 
        // because MediatR is registered in the Shared layer scanning this assembly.
        // But we keep this method for future Validators or specific Application Services.

        return services;
    }
}