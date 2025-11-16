using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voucher.Application.Dtos;
using Voucher.Infrastructure.Data.Entities;
using Voucher.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Voucher.Application.Abstractions;

namespace Voucher.Infrastructure.Repositories;
public class QueryRepository : IQueryRepository
{
    private readonly VoucherReadDbContext _db;
    public QueryRepository(VoucherReadDbContext db) => _db = db;

   

    public async Task<IReadOnlyList<VoucherDto>> GetAllAsync(CancellationToken ct)
        => (await _db.Vouchers.AsNoTracking().ToListAsync(ct)).Adapt<List<VoucherDto>>();

    public async Task<VoucherDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var v = await _db.Vouchers.AsNoTracking()
                                .FirstOrDefaultAsync(x => x.VoucherId == id, ct);
        return v?.Adapt<VoucherDto>();
    }
}
