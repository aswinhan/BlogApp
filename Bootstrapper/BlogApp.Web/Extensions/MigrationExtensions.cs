namespace BlogApp.Web.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        // 1. Migrate Identity Module
        using IdentityDbContext identityContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        ApplyMigrationSafe(identityContext);

        // 2. Migrate Blog Module
        using BlogDbContext blogContext = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
        ApplyMigrationSafe(blogContext);
    }

    private static void ApplyMigrationSafe(DbContext context)
    {
        try
        {
            int retries = 5;
            while (retries > 0)
            {
                try
                {
                    // Accessing the instance 'context', NOT the class 'DbContext'
                    context.Database.Migrate();
                    break;
                }
                catch (NpgsqlException)
                {
                    retries--;
                    if (retries == 0) throw;
                    Thread.Sleep(2000);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Migration Failed: {ex.Message}");
            throw;
        }
    }
}