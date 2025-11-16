using MassTransit;
using Voucher.Infrastructure.Data;
using Voucher.Shared.Events;
using Microsoft.Extensions.Logging;

namespace Voucher.Messaging.Consumers.QueryConsumers;

public class VoucherUsageIncreasedConsumer : IConsumer<VoucherUsageIncreasedEvent>
{
    private readonly VoucherReadDbContext _context;
    private readonly ILogger<VoucherUsageIncreasedConsumer> _logger;

    public VoucherUsageIncreasedConsumer(
        VoucherReadDbContext context,
        ILogger<VoucherUsageIncreasedConsumer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<VoucherUsageIncreasedEvent> context)
    {
        var message = context.Message;

        var voucher = await _context.Vouchers.FindAsync(message.VoucherId);

        if (voucher == null)
        {
            _logger.LogWarning("Voucher {VoucherId} not found.", message.VoucherId);
            return;
        }

        voucher.Quantity = message.Quantity;
        voucher.UsedCount = message.UsedCount;

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Updated Quantity={Quantity}, UsedCount={UsedCount} for Voucher {VoucherId}",
            message.Quantity, message.UsedCount, message.VoucherId);
    }
}
