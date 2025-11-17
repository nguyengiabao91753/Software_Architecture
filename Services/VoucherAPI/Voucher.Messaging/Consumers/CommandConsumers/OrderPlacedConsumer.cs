using Integrations.Messaging.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Voucher.Infrastructure.Data;
using Voucher.Shared.Events;

namespace Voucher.Messaging.Consumers.CommandConsumers;

public class OrderPlacedConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly WriteDbContext _db;
    private readonly IPublishEndpoint _publish;

    public OrderPlacedConsumer(WriteDbContext db, IPublishEndpoint publish)
    {
        _db = db;
        _publish = publish;
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

        // Update WriteDB
        voucher.Quantity -= 1;
        voucher.UsedCount += 1;
        await _db.SaveChangesAsync();

        // ⭐⭐⭐ PUBLISH EVENT TO READ SIDE ⭐⭐⭐
        var evt = new VoucherUsageIncreasedEvent(
            voucher.VoucherId,
            voucher.Quantity,
            voucher.UsedCount,
            DateTime.UtcNow
        );

        await _publish.Publish(evt);
    }
}
