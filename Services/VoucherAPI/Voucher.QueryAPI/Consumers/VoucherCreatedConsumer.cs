using MassTransit;
using Microsoft.EntityFrameworkCore;
using Voucher.Infrastructure.Data;
using Voucher.Infrastructure.Data.Entities;
using Voucher.Shared.Events;

namespace Voucher.QueryAPI.Consumers;

public class VoucherCreatedConsumer : IConsumer<VoucherCreatedEvent>
{
    private readonly VoucherReadDbContext _db; // ✅ Đổi tên class

    public VoucherCreatedConsumer(VoucherReadDbContext db) // ✅
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<VoucherCreatedEvent> context)
    {
        var message = context.Message;

        Console.WriteLine($"[Voucher.QueryAPI] Received VoucherCreatedEvent: " +
                          $"{message.VoucherCode} - {message.Description}");

        var exists = await _db.Vouchers
            .AsNoTracking()
            .AnyAsync(v => v.VoucherId == message.VoucherId);

        if (exists)
        {
            Console.WriteLine($"[Voucher.QueryAPI] Voucher {message.VoucherCode} đã tồn tại, bỏ qua.");
            return;
        }

        var voucher = new VoucherEntity
        {
            VoucherId = message.VoucherId,
            VoucherCode = message.VoucherCode,
            Description = message.Description,
            DiscountType = message.DiscountType,
            DiscountValue = message.DiscountValue,
            StartDate = message.StartDate,
            EndDate = message.EndDate,
            Quantity = message.Quantity,
            UsedCount = 0,
            Status = "active"
        };

        _db.Vouchers.Add(voucher);
        await _db.SaveChangesAsync();

        Console.WriteLine($"[Voucher.QueryAPI] Voucher {message.VoucherCode} đã được ghi vào ReadDB.");
    }
}
