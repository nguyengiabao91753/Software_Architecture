using MediatR;
using Voucher.Application.Dtos;

namespace Voucher.Application.Commands.CreateVoucher;

public record CreateVoucherCommand(VoucherDto Voucher) : IRequest<Guid>;
