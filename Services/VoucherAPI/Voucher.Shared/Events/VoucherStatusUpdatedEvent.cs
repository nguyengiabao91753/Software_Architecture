namespace Voucher.Shared.Events;

public record VoucherStatusUpdatedEvent(
    Guid VoucherId,
    string Status,
    DateTime OccurredAt
);
