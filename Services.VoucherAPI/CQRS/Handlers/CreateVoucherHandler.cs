using MediatR;
using Services.VoucherAPI.Data;
using Services.VoucherAPI.Models;
using Services.VoucherAPI.CQRS.Commands;

namespace Services.VoucherAPI.CQRS.Handlers
{
    public class CreateVoucherHandler : IRequestHandler<CreateVoucherCommand, Voucher>
    {
        private readonly VoucherDbContext _db;

        public CreateVoucherHandler(VoucherDbContext db)
        {
            _db = db;
        }

        public async Task<Voucher> Handle(CreateVoucherCommand request, CancellationToken cancellationToken)
        {
            var voucher = new Voucher
            {
                VoucherCode = request.VoucherCode,
                Description = request.Description,
                DiscountType = request.DiscountType,
                DiscountValue = request.DiscountValue,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Quantity = request.Quantity,
                UsedCount = 0,
                Status = "active"
            };

            _db.Vouchers.Add(voucher);
            await _db.SaveChangesAsync(cancellationToken);
            return voucher;
        }
    }
}
