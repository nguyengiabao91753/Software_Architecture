namespace Voucher.Shared.Events;

public record VoucherUsageIncreasedEvent(
    Guid VoucherId,
    int UsedCount,
    DateTime OccurredAt
);
