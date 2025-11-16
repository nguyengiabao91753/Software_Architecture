using Carter;
using MediatR;
using Voucher.Application.Commands.IncreaseUsage;

namespace Voucher.CommandAPI.Endpoints;

public class IncreaseUsage : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/vouchers/{id}/use", async (Guid id, ISender sender) =>
        {
            var success = await sender.Send(new IncreaseUsageCommand(id));
            return success
                ? Results.Ok("Usage count increased successfully")
                : Results.NotFound("Voucher not found");
        })
        .WithName("IncreaseUsage")
        .WithSummary("Tăng lượt sử dụng voucher (WriteDB)")
        .WithDescription("Tăng UsedCount của voucher lên 1 trong cơ sở dữ liệu ghi (WriteDB).");
    }
}
