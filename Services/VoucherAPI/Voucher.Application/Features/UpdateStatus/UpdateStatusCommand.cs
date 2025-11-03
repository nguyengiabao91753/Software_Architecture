using MediatR;

namespace Voucher.Application.Features.UpdateStatus;

public record UpdateStatusCommand(Guid VoucherId, string Status) : IRequest<bool>;
