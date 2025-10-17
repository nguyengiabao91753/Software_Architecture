using System.ComponentModel.DataAnnotations;

namespace Services.VoucherAPI.Models;

public class Voucher
{
    [Key]
    public Guid VoucherId { get; set; } = Guid.NewGuid();

    [Required, MaxLength(50)]
    public string VoucherCode { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public string DiscountType { get; set; } = "percent"; // percent | fixed

    [Required]
    public decimal DiscountValue { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public int Quantity { get; set; }
    public int UsedCount { get; set; } = 0;

    public string Status { get; set; } = "active";
}
