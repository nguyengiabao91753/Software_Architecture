using Carter;
using MediatR;
using Voucher.Application.Commands.UpdateStatus;

namespace Voucher.CommandAPI.Endpoints;

public class UpdateStatus : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/vouchers/{id}/status", async (Guid id, UpdateStatusRequest request, ISender sender) =>
        {
            var success = await sender.Send(new UpdateStatusCommand(id, request.Status));
            return success
                ? Results.Ok($"Voucher status updated to '{request.Status}' successfully")
                : Results.NotFound("Voucher not found");
        })
        .WithName("UpdateStatus")
        .WithSummary("Cập nhật trạng thái voucher (WriteDB)")
        .WithDescription("Thay đổi trạng thái voucher giữa 'active' và 'inactive' trong cơ sở dữ liệu ghi (WriteDB).");
    }
}

public record UpdateStatusRequest(string Status);
