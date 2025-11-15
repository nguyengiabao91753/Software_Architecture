using Integrations.Messaging.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Voucher.Infrastructure.Data;

namespace Voucher.Messaging.Consumers;

public class OrderPlacedConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly WriteDbContext _db;

    public OrderPlacedConsumer(WriteDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        var msg = context.Message;

        if (msg.VoucherId == null)
            return;
        var voucher = await _db.Vouchers
            .FirstOrDefaultAsync(v => v.VoucherId == msg.VoucherId);

        if (voucher == null)
            return;

        voucher.Quantity -= 1;
        voucher.UsedCount += 1;

        await _db.SaveChangesAsync();
    }
}
