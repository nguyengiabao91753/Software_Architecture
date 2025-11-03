using MediatR;
using Voucher.Application.Dtos;

namespace Voucher.Application.Features.GetVouchers;

public record GetVouchersQuery() : IRequest<IReadOnlyList<VoucherDto>>;
