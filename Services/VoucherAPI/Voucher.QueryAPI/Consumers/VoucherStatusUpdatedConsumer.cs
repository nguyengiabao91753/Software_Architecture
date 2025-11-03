using MassTransit;
using Voucher.Infrastructure.Data;
using Voucher.Shared.Events;

namespace Voucher.QueryAPI.Consumers;

public class VoucherStatusUpdatedConsumer : IConsumer<VoucherStatusUpdatedEvent>
{
    private readonly VoucherReadDbContext _context;
    private readonly ILogger<VoucherStatusUpdatedConsumer> _logger;

    public VoucherStatusUpdatedConsumer(VoucherReadDbContext context, ILogger<VoucherStatusUpdatedConsumer> logger) // ✅
    {
        _context = context;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<VoucherStatusUpdatedEvent> context)
    {
        var message = context.Message;
        var voucher = await _context.Vouchers.FindAsync(message.VoucherId);

        if (voucher == null)
        {
            _logger.LogWarning("Voucher {VoucherId} not found in ReadDB.", message.VoucherId);
            return;
        }

        voucher.Status = message.Status;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated Status='{Status}' for Voucher {VoucherId}",
            message.Status, message.VoucherId);
    }
}
