using MediatR;
using MassTransit;
using Voucher.Application.Abstractions;
using Voucher.Shared.Events;

namespace Voucher.Application.Commands.IncreaseUsage;

public class IncreaseUsageHandler : IRequestHandler<IncreaseUsageCommand, bool>
{
    private readonly IVoucherRepository _repo;
    private readonly IPublishEndpoint _publishEndpoint;

    public IncreaseUsageHandler(IVoucherRepository repo, IPublishEndpoint publishEndpoint)
    {
        _repo = repo;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<bool> Handle(IncreaseUsageCommand request, CancellationToken cancellationToken)
    {
        // Update WriteDB
        var success = await _repo.IncreaseUsageAsync(request.VoucherId, cancellationToken);
        if (!success)
            return false;

        var voucher = await _repo.GetByIdAsync(request.VoucherId, cancellationToken);
        if (voucher == null)
            return false;

        await _publishEndpoint.Publish(new VoucherUsageIncreasedEvent(
            voucher.VoucherId,
            voucher.Quantity,
            voucher.UsedCount,
            DateTime.UtcNow
        ), cancellationToken);

        Console.WriteLine($"Published VoucherUsageIncreasedEvent for {voucher.VoucherCode}");

        return true;
    }
}
