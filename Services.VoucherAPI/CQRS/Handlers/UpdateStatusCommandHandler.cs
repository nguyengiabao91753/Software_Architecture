using MediatR;
using Services.VoucherAPI.Data;
using Services.VoucherAPI.Models;
using Services.VoucherAPI.CQRS.Commands;

namespace Services.VoucherAPI.CQRS.Handlers
{
    public class UpdateStatusCommandHandler : IRequestHandler<UpdateStatusCommand, Voucher>
    {
        private readonly VoucherDbContext _db;

        public UpdateStatusCommandHandler(VoucherDbContext db)
        {
            _db = db;
        }

        public async Task<Voucher> Handle(UpdateStatusCommand request, CancellationToken cancellationToken)
        {
            var voucher = await _db.Vouchers.FindAsync(request.VoucherId);
            if (voucher == null)
                throw new Exception("Voucher not found");

            voucher.Status = request.Status;
            await _db.SaveChangesAsync(cancellationToken);
            return voucher;
        }
    }
}
