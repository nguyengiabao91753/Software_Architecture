using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Voucher.Infrastructure.Data.Extensions;

public static class Initialiser
{
    // Migration cho CommandAPI (WriteDB)
    public static async Task InitialiseWriteDbAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var writeDb = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

        await writeDb.Database.MigrateAsync();
        Console.WriteLine("WriteDB migrated successfully.");
    }

    // KHÔNG MIGRATE QueryDB — chỉ EnsureCreated()
    public static async Task InitialiseReadDbAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var readDb = scope.ServiceProvider.GetRequiredService<VoucherReadDbContext>();

        // ❗ ReadDB chỉ cần auto-create schema
        await readDb.Database.EnsureCreatedAsync();
        Console.WriteLine("ReadDB ensured/created successfully.");
    }
}
