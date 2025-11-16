using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voucher.Application.Dtos;

namespace Voucher.Application.Abstractions;
public interface IQueryRepository
{
    Task<IReadOnlyList<VoucherDto>> GetAllAsync(CancellationToken ct);

    Task<VoucherDto?> GetByIdAsync(Guid id, CancellationToken ct);


}
