using MediatR;
using Voucher.Application.Abstractions;
using Voucher.Application.Dtos;

namespace Voucher.Application.Queries.GetVouchers;

public class GetVouchersHandler : IRequestHandler<GetVouchersQuery, IReadOnlyList<VoucherDto>>
{
    private readonly IVoucherRepository _repo;

    public GetVouchersHandler(IVoucherRepository repo)
    {
        _repo = repo;
    }

    public async Task<IReadOnlyList<VoucherDto>> Handle(GetVouchersQuery request, CancellationToken cancellationToken)
    {
        return await _repo.GetAllAsync(cancellationToken);
    }
}
