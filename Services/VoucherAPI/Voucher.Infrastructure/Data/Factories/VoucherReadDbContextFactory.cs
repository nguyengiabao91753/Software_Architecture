using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Voucher.Infrastructure.Data.Factories;

public class VoucherReadDbContextFactory : IDesignTimeDbContextFactory<VoucherReadDbContext>
{
    public VoucherReadDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<VoucherReadDbContext>();
        // Cập nhật đúng connection string của bạn
        optionsBuilder.UseSqlServer(
            "Server=DESKTOP-H4R8V0T;Database=VoucherReadDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;MultipleActiveResultSets=True;"
        );

        return new VoucherReadDbContext(optionsBuilder.Options);
    }
}
