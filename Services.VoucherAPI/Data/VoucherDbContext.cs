using Microsoft.EntityFrameworkCore;
using Services.VoucherAPI.Models;

namespace Services.VoucherAPI.Data;

public class VoucherDbContext : DbContext
{
    public VoucherDbContext(DbContextOptions<VoucherDbContext> options)
        : base(options) { }

    public DbSet<Voucher> Vouchers => Set<Voucher>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Voucher>().HasIndex(v => v.VoucherCode).IsUnique();
        modelBuilder.Entity<Voucher>()
            .Property(v => v.Status)
            .HasDefaultValue("active");
    }
}
