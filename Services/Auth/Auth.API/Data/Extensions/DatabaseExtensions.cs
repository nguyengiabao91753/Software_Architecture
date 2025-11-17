using Microsoft.EntityFrameworkCore;

namespace Auth.API.Data.Extensions;

public static class DatabaseExtensions
{
    public static async Task InitialiseDatabaseAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IdentityApplicationDbContext>();

        await context.Database.MigrateAsync();

    }
}
