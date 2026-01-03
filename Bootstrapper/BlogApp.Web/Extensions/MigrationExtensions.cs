namespace BlogApp.Web.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        using IdentityDbContext dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        try
        {
            // Simple retry logic
            int retries = 5;
            while (retries > 0)
            {
                try
                {
                    dbContext.Database.Migrate();
                    break; // Success! Exit loop.
                }
                catch (NpgsqlException)
                {
                    retries--;
                    if (retries == 0) throw; // Re-throw if we run out of tries

                    // Wait 2 seconds before retrying
                    Thread.Sleep(2000);
                }
            }
        }
        catch (Exception ex)
        {
            // Log the error (optional, but good for debugging)
            Console.WriteLine($"Migration Failed: {ex.Message}");
            throw;
        }
    }
}