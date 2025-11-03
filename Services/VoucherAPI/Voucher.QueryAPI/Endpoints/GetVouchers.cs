using Carter;
using MediatR;
using Voucher.Application.Features.GetVouchers;

namespace Voucher.QueryAPI.Endpoints; // ✅ đổi namespace

public class GetVouchers : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/vouchers", async (ISender sender) =>
        {
            var list = await sender.Send(new GetVouchersQuery());
            return Results.Ok(list);
        })
        .WithName("GetVouchers")
        .WithSummary("Lấy danh sách tất cả voucher")
        .WithDescription("Trả về toàn bộ danh sách voucher trong hệ thống (ReadDB).");
    }
}
