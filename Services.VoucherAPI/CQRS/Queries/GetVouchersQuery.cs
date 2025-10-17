using MediatR;
using Services.VoucherAPI.Models;

namespace Services.VoucherAPI.CQRS.Queries
{
    // Lấy toàn bộ danh sách voucher
    public record GetVouchersQuery() : IRequest<List<Voucher>>;
}
