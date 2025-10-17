using MediatR;
using Services.VoucherAPI.Models;

namespace Services.VoucherAPI.CQRS.Commands
{
    public record CreateVoucherCommand(
        string VoucherCode,
        string Description,
        string DiscountType,
        decimal DiscountValue,
        DateTime StartDate,
        DateTime EndDate,
        int Quantity
    ) : IRequest<Voucher>;
}
