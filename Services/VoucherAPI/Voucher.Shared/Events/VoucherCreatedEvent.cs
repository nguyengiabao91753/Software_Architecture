namespace Voucher.Shared.Events;

public record VoucherCreatedEvent(
    Guid VoucherId,
    string VoucherCode,
    string Description,
    string DiscountType,
    decimal DiscountValue,
    DateTime StartDate,
    DateTime EndDate,
    int Quantity,
    DateTime CreatedAt
);
