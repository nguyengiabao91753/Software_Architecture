
using Microsoft.EntityFrameworkCore;
using Orders.Application.Data;
using Orders.Application.Dtos;
using Orders.Application.IServices;
using Orders.Application.Mapping;
using Orders.Domain.Models;
using Orders.Domain.ValueObjects;

namespace Services.OrderAPI.Application.Services;

public class OrderService : IOrderService
{
    private readonly IApplicationDbContext _db;

    public OrderService(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ResultService<OrderDto>> GetById(Guid id)
    {
        var rs = new ResultService<OrderDto>();
        try
        {
            var entity = OrderMappingData.ToOrderDto(await _db.Orders.FindAsync(id));
            if (entity == null)
            {
                rs.IsSuccess = false;
                rs.Message = "Order not found";
                return rs;
            }
            rs.Data = entity;
            rs.IsSuccess = true;


        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = ex.Message;
        }
        return rs;
    }

    public async Task<ResultService<OrderDto>> GetByTrackingId(Guid trackingId)
    {
       var rs = new ResultService<OrderDto>();
        try
        {
            var entity = OrderMappingData.ToOrderDto(await _db.Orders.FirstOrDefaultAsync(o => o.TrackingId.Value == trackingId));
            if (entity == null)
            {
                rs.IsSuccess = false;
                rs.Message = "Order not found";
                return rs;
            }
            rs.Data = entity;
            rs.IsSuccess = true;
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = ex.Message;
        }
        return rs;
    }

    public async Task<ResultService<OrderDto>> Save(OrderDto order)
    {
        var rs = new ResultService<OrderDto>();
        using (var transaction = _db.Database.BeginTransaction())
        {
            try
            {
                var newOrder = Order.CreateOrder(
                    customerId: order.CustomerId,
                    restaurantId: order.RestaurantId,
                    trackingId: order.TrackingId,
                    voucherId: order.VoucherId

                );
                foreach(var item in order.OrderItems)
                {
                    newOrder.AddOrderItem(
                        productId: ProductId.Of(item.ProductId),
                        price: item.Price,
                        quantity: item.Quantity
                    );
                }

                await _db.Orders.AddAsync(newOrder);
                await _db.SaveChangesAsync();
                
                transaction.Commit();
                rs.Data = OrderMappingData.ToOrderDto(newOrder);
                rs.IsSuccess = true;
                rs.Message = "Order created successfully";

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                rs.IsSuccess = false;
                rs.Message = ex.Message;
            }
        }
        return rs;
    }

    public Task<ResultService<OrderDto>> Update(OrderDto order)
    {
        throw new NotImplementedException();
    }
}
