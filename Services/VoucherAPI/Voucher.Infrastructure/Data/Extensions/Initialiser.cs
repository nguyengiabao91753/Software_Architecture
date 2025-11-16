using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Voucher.Infrastructure.Data;

namespace Voucher.Infrastructure.Data.Extensions;

public static class Initialiser
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var writeDb = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
        var readDb = scope.ServiceProvider.GetRequiredService<VoucherReadDbContext>();

        await writeDb.Database.MigrateAsync();
        await readDb.Database.MigrateAsync();

        Console.WriteLine("✅ Databases migrated successfully (WriteDB + ReadDB).");
    }
}
