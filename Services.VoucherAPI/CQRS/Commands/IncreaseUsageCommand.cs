using MediatR;
using Services.VoucherAPI.Models;

namespace Services.VoucherAPI.CQRS.Commands
{
    // Command: Tăng lượt sử dụng (UsedCount) của voucher
    public record IncreaseUsageCommand(Guid VoucherId) : IRequest<Voucher>;
}
