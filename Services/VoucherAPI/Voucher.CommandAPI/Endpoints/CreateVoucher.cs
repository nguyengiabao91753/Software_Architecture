using Carter;
using MediatR;
using Voucher.Application.Dtos;
using Voucher.Application.Features.CreateVoucher;

namespace Voucher.CommandAPI.Endpoints; // ✅ Đổi namespace cho đúng

public class CreateVoucher : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/vouchers", async (VoucherDto dto, ISender sender) =>
        {
            var id = await sender.Send(new CreateVoucherCommand(dto));
            return Results.Created($"/vouchers/{id}", new { id });
        })
        .WithName("CreateVoucher")
        .WithSummary("Tạo mới voucher")
        .WithDescription("Thêm voucher mới vào hệ thống (WriteDB).");
    }
}
