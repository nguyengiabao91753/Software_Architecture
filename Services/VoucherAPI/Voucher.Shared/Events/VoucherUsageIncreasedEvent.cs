namespace Voucher.Shared.Events;

public record VoucherUsageIncreasedEvent(
    Guid VoucherId,
    int Quantity,
    int UsedCount,
    DateTime OccurredAt
);
