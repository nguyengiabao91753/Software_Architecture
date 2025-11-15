using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Domain.Enums;
using Orders.Domain.Models;
using Orders.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Infrastructure.Data.Configurations;
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasConversion(
            id => id.Value,
            value => OrderId.Of(value)
        );

        builder.Property(o => o.CustomerId).HasConversion(
            id => id.Value,
            value => CustomerId.Of(value)
        );
        builder.Property(o => o.TrackingId).HasConversion(
            id => id.Value,
            value => TrackingId.Of(value)
        );
        builder.Property(o => o.VoucherId).HasConversion(
            id => id.Value,
            value => value == null ? null : VoucherId.Of(value)
        ).IsRequired(false);
        builder.Property(o => o.RestaurantId).HasConversion(
            id => id.Value,
            value => RestaurantId.Of(value)
        );

        builder.Property(o => o.OrderStatus)
            .HasDefaultValue(OrderStatus.Pending)
            .HasConversion(
                os => os.ToString(),
                value => (OrderStatus)Enum.Parse(typeof(OrderStatus), value)
            );
        builder.Property(o => o.FailureMessages).HasMaxLength(500);
        builder.Ignore(o => o.TotalAmount);

        builder.HasMany(o => o.OrderItems)
            .WithOne()
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
