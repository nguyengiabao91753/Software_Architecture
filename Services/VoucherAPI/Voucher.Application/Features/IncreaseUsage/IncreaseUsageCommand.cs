using MediatR;

namespace Voucher.Application.Features.IncreaseUsage;

public record IncreaseUsageCommand(Guid VoucherId) : IRequest<bool>;
