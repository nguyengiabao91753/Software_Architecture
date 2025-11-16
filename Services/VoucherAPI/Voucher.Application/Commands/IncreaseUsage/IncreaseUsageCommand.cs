using MediatR;

namespace Voucher.Application.Commands.IncreaseUsage;

public record IncreaseUsageCommand(Guid VoucherId) : IRequest<bool>;
