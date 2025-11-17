using MediatR;
using Voucher.Application.Dtos;

namespace Voucher.Application.Queries.GetVouchers;

public record GetVouchersQuery() : IRequest<IReadOnlyList<VoucherDto>>;
