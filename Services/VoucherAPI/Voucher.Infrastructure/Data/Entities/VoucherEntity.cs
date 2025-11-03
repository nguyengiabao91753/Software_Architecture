namespace Voucher.Infrastructure.Data.Entities;

public class VoucherEntity
{
    public Guid VoucherId { get; set; }
    public string VoucherCode { get; set; } = default!;
    public string? Description { get; set; }
    public string DiscountType { get; set; } = default!; // 'percent' | 'fixed'
    public decimal DiscountValue { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Quantity { get; set; }
    public int UsedCount { get; set; } = 0;
    public string Status { get; set; } = "active";
}
