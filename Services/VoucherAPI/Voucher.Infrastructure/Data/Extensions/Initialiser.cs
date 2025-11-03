using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Voucher.Infrastructure.Data.Extensions;

public static class Initialiser
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var writeDb = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        await writeDb.Database.MigrateAsync();

        var readDb = scope.ServiceProvider.GetRequiredService<VoucherReadDbContext>();
        await readDb.Database.MigrateAsync();
    }
}
