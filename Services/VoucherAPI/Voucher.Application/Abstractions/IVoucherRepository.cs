using Voucher.Application.Dtos;

namespace Voucher.Application.Abstractions;

public interface IVoucherRepository
{
    Task<Guid> CreateAsync(VoucherDto dto, CancellationToken ct);
    Task<IReadOnlyList<VoucherDto>> GetAllAsync(CancellationToken ct);
    Task<bool> IncreaseUsageAsync(Guid id, CancellationToken ct);
    Task<bool> UpdateStatusAsync(Guid id, string status, CancellationToken ct);

    // Thêm dòng này để fix lỗi build
    Task<VoucherDto?> GetByIdAsync(Guid id, CancellationToken ct);
}
