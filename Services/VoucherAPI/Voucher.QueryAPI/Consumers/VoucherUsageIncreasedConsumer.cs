using MassTransit;
using Voucher.Infrastructure.Data;
using Voucher.Shared.Events;

namespace Voucher.QueryAPI.Consumers;

public class VoucherUsageIncreasedConsumer : IConsumer<VoucherUsageIncreasedEvent>
{
    private readonly VoucherReadDbContext _context; // ✅
    private readonly ILogger<VoucherUsageIncreasedConsumer> _logger;

    public VoucherUsageIncreasedConsumer(VoucherReadDbContext context, ILogger<VoucherUsageIncreasedConsumer> logger) // ✅
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
            _logger.LogWarning("Voucher {VoucherId} not found in ReadDB.", message.VoucherId);
            return;
        }

        voucher.UsedCount = message.UsedCount;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated UsedCount={UsedCount} for Voucher {VoucherId}",
            message.UsedCount, message.VoucherId);
    }
}
