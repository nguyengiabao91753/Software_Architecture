
using Integrations.Messaging.Events;
using MassTransit;
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
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderService(IApplicationDbContext db, IPublishEndpoint eventPublisher)
    {
        _db = db;
        _publishEndpoint = eventPublisher;
    }

    public async Task<ResultService<List<OrderDto>>> GetAllOrders()
    {
        
        var rs = new ResultService<List<OrderDto>>();
        try
        {
            var orders = await _db.Orders
                .Include(o => o.OrderItems)
                .ToListAsync();
            rs.Data = orders.Select(OrderMappingData.ToOrderDto).ToList();
            rs.IsSuccess = true;
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = ex.Message;
        }
        return rs;
    }

    public async Task<ResultService<OrderDto>> GetById(Guid id)
    {
        var rs = new ResultService<OrderDto>();
        try
        {
            var order = await _db.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == OrderId.Of(id));

            if (order == null)
            {
                rs.IsSuccess = false;
                rs.Message = "Order not found";
                return rs;
            }

            rs.Data = OrderMappingData.ToOrderDto(order);
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



                var orderPlcedEvent = new OrderPlacedEvent
                {
                    OrderId = newOrder.Id.Value,
                    CustomerId = newOrder.CustomerId.Value,
                    RestaurantId = newOrder.RestaurantId.Value,
                    TrackingId = newOrder.TrackingId.Value,
                    VoucherId = newOrder.VoucherId?.Value,
                    TotalAmount = newOrder.TotalAmount,
                    OrderItems = newOrder.OrderItems.Select(oi => new OrderItemEvent
                    {
                        ProductId = oi.ProductId.Value,
                        Quantity = oi.Quantity,
                        Price = oi.Price,
                        SubTotal = oi.SubTotal
                    }).ToList()
                };

                await _publishEndpoint.Publish(orderPlcedEvent);

               

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

    public async Task<ResultService<OrderDto>> Update(OrderDto order)
    {
        var rs = new ResultService<OrderDto>();
        using (var transaction = _db.Database.BeginTransaction())
        {
            try
            {
                var oldOrder = _db.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefault(o => o.Id == OrderId.Of(order.Id));

                oldOrder!.UpdateOrderStatus(order.OrderStatus);

                
                await _db.SaveChangesAsync();


                transaction.Commit();
                rs.Data = OrderMappingData.ToOrderDto(oldOrder);
                rs.IsSuccess = true;
                rs.Message = "Order updated successfully";

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
}
