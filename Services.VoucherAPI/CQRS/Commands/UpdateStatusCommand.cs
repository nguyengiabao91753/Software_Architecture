using MediatR;
using Services.VoucherAPI.Models;

namespace Services.VoucherAPI.CQRS.Commands
{
    // Command: Cập nhật trạng thái voucher (active / inactive)
    public record UpdateStatusCommand(Guid VoucherId, string Status) : IRequest<Voucher>;
}
