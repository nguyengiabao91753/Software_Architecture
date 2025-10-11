using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Services.OrderAPI.Domain.Entities;

namespace Services.OrderAPI.Infrastructure.DataBase;

public partial class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ================================
        // Orders
        // ================================
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK_Orders");

            entity.HasIndex(e => e.CustomerId)
                .HasDatabaseName("IX_Orders_CustomerId");

            entity.HasIndex(e => e.TrackingId)
                .HasDatabaseName("IX_Orders_TrackingId")
                .IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()") // DB sinh tự động
                .ValueGeneratedOnAdd();

            entity.Property(e => e.OrderStatus)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasCheckConstraint(
                "CK_Orders_OrderStatus",
                "OrderStatus IN ('PENDING','PAID','APPROVED','CANCELLING','CANCELLED')"
);

            entity.Property(e => e.Price)
                .HasColumnType("decimal(10,2)");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("SYSDATETIMEOFFSET()")
                .ValueGeneratedOnAdd();
        });

        // ================================
        // OrderItems
        // ================================
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id)
                .HasName("PK_OrderItems");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("NEWSEQUENTIALID()")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Price)
                .HasColumnType("decimal(10,2)");

            entity.Property(e => e.SubTotal)
                .HasColumnType("decimal(10,2)");

            entity.HasOne(d => d.Order)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_OrderItems_Orders")
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
