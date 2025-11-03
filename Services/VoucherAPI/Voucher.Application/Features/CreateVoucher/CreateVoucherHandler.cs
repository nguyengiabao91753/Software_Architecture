using MassTransit;
using MediatR;
using Voucher.Application.Abstractions;
using Voucher.Shared.Events;

namespace Voucher.Application.Features.CreateVoucher;

public class CreateVoucherHandler : IRequestHandler<CreateVoucherCommand, Guid>
{
    private readonly IVoucherRepository _repo;
    private readonly IPublishEndpoint _publishEndpoint; // ✅ để publish event

    public CreateVoucherHandler(IVoucherRepository repo, IPublishEndpoint publishEndpoint)
    {
        _repo = repo;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Guid> Handle(CreateVoucherCommand request, CancellationToken cancellationToken)
    {
        // Ghi vào WriteDB
        var newVoucherId = await _repo.CreateAsync(request.Voucher, cancellationToken);

        // Tạo event để publish
        var evt = new VoucherCreatedEvent(
            newVoucherId,
            request.Voucher.VoucherCode,
            request.Voucher.Description ?? string.Empty,
            request.Voucher.DiscountType,
            request.Voucher.DiscountValue,
            request.Voucher.StartDate,
            request.Voucher.EndDate,
            request.Voucher.Quantity,
            DateTime.UtcNow
        );

        // Publish lên RabbitMQ
        await _publishEndpoint.Publish(evt, cancellationToken);

        Console.WriteLine($"Published VoucherCreatedEvent: {evt.VoucherCode}");

        // Trả về ID mới tạo
        return newVoucherId;
    }
}
