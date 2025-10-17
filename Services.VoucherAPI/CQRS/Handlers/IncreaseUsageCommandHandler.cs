using MediatR;
using Services.VoucherAPI.Data;
using Services.VoucherAPI.Models;
using Services.VoucherAPI.CQRS.Commands;

namespace Services.VoucherAPI.CQRS.Handlers
{
    public class IncreaseUsageCommandHandler : IRequestHandler<IncreaseUsageCommand, Voucher>
    {
        private readonly VoucherDbContext _db;

        public IncreaseUsageCommandHandler(VoucherDbContext db)
        {
            _db = db;
        }

        public async Task<Voucher> Handle(IncreaseUsageCommand request, CancellationToken cancellationToken)
        {
            var voucher = await _db.Vouchers.FindAsync(request.VoucherId);
            if (voucher == null)
                throw new Exception("Voucher not found");

            if (voucher.UsedCount >= voucher.Quantity)
                throw new Exception("Voucher usage limit reached");

            voucher.UsedCount++;
            await _db.SaveChangesAsync(cancellationToken);
            return voucher;
        }
    }
}
