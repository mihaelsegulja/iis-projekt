using IISNotionSearch.Repository;
using Microsoft.EntityFrameworkCore;

namespace IISNotionSearch.API.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IHost app) 
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
    }
}