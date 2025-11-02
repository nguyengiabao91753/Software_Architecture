using MediatR;
using Voucher.Application.Dtos;

namespace Voucher.Application.Features.CreateVoucher;

public record CreateVoucherCommand(VoucherDto Voucher) : IRequest<Guid>;
