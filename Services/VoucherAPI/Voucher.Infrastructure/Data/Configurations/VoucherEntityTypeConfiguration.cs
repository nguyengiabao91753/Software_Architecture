using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voucher.Infrastructure.Data.Entities;

namespace Voucher.Infrastructure.Data.Configurations;

public class VoucherEntityTypeConfiguration : IEntityTypeConfiguration<VoucherEntity>
{
    public void Configure(EntityTypeBuilder<VoucherEntity> b)
    {
        b.ToTable("Vouchers");

        b.HasKey(x => x.VoucherId);
        b.HasIndex(x => x.VoucherCode).IsUnique();

        b.Property(x => x.DiscountValue)
            .HasColumnType("decimal(10,2)");

        b.Property(x => x.Status)
            .HasDefaultValue("active");

        b.Property(x => x.DiscountType)
            .HasMaxLength(10);

        b.Property(x => x.VoucherCode)
            .HasMaxLength(50);
    }
}
