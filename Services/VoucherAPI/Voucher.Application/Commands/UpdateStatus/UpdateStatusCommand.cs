using MediatR;

namespace Voucher.Application.Commands.UpdateStatus;

public record UpdateStatusCommand(Guid VoucherId, string Status) : IRequest<bool>;
