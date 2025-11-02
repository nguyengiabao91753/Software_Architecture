using Microsoft.EntityFrameworkCore;
using Voucher.Infrastructure.Data.Configurations;
using Voucher.Infrastructure.Data.Entities;

namespace Voucher.Infrastructure.Data;

public class VoucherReadDbContext : DbContext
{
    public VoucherReadDbContext(DbContextOptions<VoucherReadDbContext> options)
        : base(options) { }

    public DbSet<VoucherEntity> Vouchers => Set<VoucherEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new VoucherEntityTypeConfiguration());
    }
}
