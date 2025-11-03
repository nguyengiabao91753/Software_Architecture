using Microsoft.EntityFrameworkCore;
using Voucher.Infrastructure.Data.Configurations;
using Voucher.Infrastructure.Data.Entities;

namespace Voucher.Infrastructure.Data;

public class WriteDbContext : DbContext
{
    public WriteDbContext(DbContextOptions<WriteDbContext> options) : base(options) { }

    public DbSet<VoucherEntity> Vouchers => Set<VoucherEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new VoucherEntityTypeConfiguration());
    }
}
