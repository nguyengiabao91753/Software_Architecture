using Microsoft.EntityFrameworkCore;
using Voucher.Application.Abstractions;
using Voucher.Application.Dtos;
using Voucher.Infrastructure.Data;
using Voucher.Infrastructure.Data.Entities;
using Mapster;

namespace Voucher.Infrastructure.Repositories;

public class VoucherRepository : IVoucherRepository
{
    private readonly WriteDbContext _db;
    public VoucherRepository(WriteDbContext db) => _db = db;

    public async Task<Guid> CreateAsync(VoucherDto dto, CancellationToken ct)
    {
        var entity = dto.Adapt<VoucherEntity>();
        entity.VoucherId = Guid.NewGuid();

        _db.Vouchers.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity.VoucherId;
    }

    public async Task<IReadOnlyList<VoucherDto>> GetAllAsync(CancellationToken ct)
        => (await _db.Vouchers.AsNoTracking().ToListAsync(ct)).Adapt<List<VoucherDto>>();

    public async Task<bool> IncreaseUsageAsync(Guid id, CancellationToken ct)
    {
        var v = await _db.Vouchers.FindAsync(new object[] { id }, ct);
        if (v is null) return false;

        v.UsedCount++;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> UpdateStatusAsync(Guid id, string status, CancellationToken ct)
    {
        var v = await _db.Vouchers.FindAsync(new object[] { id }, ct);
        if (v is null) return false;

        v.Status = status;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<VoucherDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var v = await _db.Vouchers.AsNoTracking()
                                .FirstOrDefaultAsync(x => x.VoucherId == id, ct);
        return v?.Adapt<VoucherDto>();
    }

}
