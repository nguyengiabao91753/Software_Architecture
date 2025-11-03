namespace Voucher.Application.Dtos;

public record VoucherDto(
    Guid VoucherId,
    string VoucherCode,
    string Description,
    string DiscountType,
    decimal DiscountValue,
    DateTime StartDate,
    DateTime EndDate,
    int Quantity,
    int UsedCount,
    string Status
);
