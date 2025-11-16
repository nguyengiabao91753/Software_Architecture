using MassTransit;
using Microsoft.EntityFrameworkCore;
using Voucher.Infrastructure.Data;
using Voucher.Infrastructure.Data.Entities;
using Voucher.Shared.Events;

namespace Voucher.QueryAPI.Consumers;

public class VoucherCreatedConsumer : IConsumer<VoucherCreatedEvent>
{
    private readonly VoucherReadDbContext _db;

    public VoucherCreatedConsumer(VoucherReadDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<VoucherCreatedEvent> context)
    {
        var message = context.Message;

        Console.WriteLine($"[QueryAPI] Received VoucherCreatedEvent: {message.VoucherCode}");

        var exists = await _db.Vouchers
            .AsNoTracking()
            .AnyAsync(v => v.VoucherId == message.VoucherId);

        if (exists)
        {
            Console.WriteLine($"[QueryAPI] Voucher {message.VoucherCode} đã tồn tại => skip");
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
            UsedCount = message.UsedCount,
            Status = message.Status
        };

        _db.Vouchers.Add(voucher);
        await _db.SaveChangesAsync();

        Console.WriteLine($"[QueryAPI] INSERTED {message.VoucherCode} vào ReadDB thành công!");
    }
}
