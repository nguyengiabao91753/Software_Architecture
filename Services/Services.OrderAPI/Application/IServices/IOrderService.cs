using Services.OrderAPI.Application.Dtos;
using Shares.Base.BaseClass;

namespace Services.OrderAPI.Application.IServices;

public interface IOrderService
{
    Task<ResultService<OrderDto>> Save(OrderDto order);

    Task<ResultService<OrderDto>> Update(OrderDto order);

    Task<ResultService<OrderDto>> GetById(Guid id);

    Task<ResultService<OrderDto>> GetByTrackingId(Guid trackingId);
}
