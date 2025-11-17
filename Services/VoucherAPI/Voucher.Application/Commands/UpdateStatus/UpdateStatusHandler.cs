using MediatR;
using MassTransit;
using Voucher.Application.Abstractions;
using Voucher.Shared.Events;

namespace Voucher.Application.Commands.UpdateStatus;

public class UpdateStatusHandler : IRequestHandler<UpdateStatusCommand, bool>
{
    private readonly IVoucherRepository _repo;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpdateStatusHandler(IVoucherRepository repo, IPublishEndpoint publishEndpoint)
    {
        _repo = repo;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<bool> Handle(UpdateStatusCommand request, CancellationToken cancellationToken)
    {
        // 1️⃣ Cập nhật trạng thái trong WriteDB
        var success = await _repo.UpdateStatusAsync(request.VoucherId, request.Status, cancellationToken);
        if (!success)
            return false;

        // 2️⃣ Lấy lại voucher sau khi cập nhật
        var voucher = await _repo.GetByIdAsync(request.VoucherId, cancellationToken);
        if (voucher == null)
            return false;

        // 3️⃣ Publish event sang RabbitMQ để cập nhật ReadDB
        await _publishEndpoint.Publish(new VoucherStatusUpdatedEvent(
            voucher.VoucherId,
            voucher.Status,
            DateTime.UtcNow
        ), cancellationToken);

        Console.WriteLine($"📤 Published VoucherStatusUpdatedEvent for {voucher.VoucherCode}");

        return true;
    }
}
