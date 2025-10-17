using MediatR;
using Microsoft.EntityFrameworkCore;
using Services.VoucherAPI.Data;
using Services.VoucherAPI.Models;
using Services.VoucherAPI.CQRS.Queries;

namespace Services.VoucherAPI.CQRS.Handlers
{
    public class GetVouchersQueryHandler : IRequestHandler<GetVouchersQuery, List<Voucher>>
    {
        private readonly VoucherDbContext _db;

        public GetVouchersQueryHandler(VoucherDbContext db)
        {
            _db = db;
        }

        public async Task<List<Voucher>> Handle(GetVouchersQuery request, CancellationToken cancellationToken)
        {
            return await _db.Vouchers.AsNoTracking().ToListAsync(cancellationToken);
        }
    }
}
