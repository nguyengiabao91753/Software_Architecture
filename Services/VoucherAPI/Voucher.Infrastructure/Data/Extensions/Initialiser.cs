using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Voucher.Infrastructure.Data;

namespace Voucher.Infrastructure.Data.Extensions;

public static class Initialiser
{
    public static async Task InitialiseWriteDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var writeDb = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

        await writeDb.Database.MigrateAsync();

        Console.WriteLine("✅ Databases migrated successfully (WriteDB).");
    }


    public static async Task InitialiseReadDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var readDb = scope.ServiceProvider.GetRequiredService<VoucherReadDbContext>();

        await readDb.Database.MigrateAsync();

        Console.WriteLine("✅ Databases migrated successfully (ReadDB).");
    }
}
