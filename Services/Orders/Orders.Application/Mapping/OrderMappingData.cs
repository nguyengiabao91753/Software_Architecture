using Orders.Application.Dtos;
using Orders.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orders.Domain.ValueObjects;
namespace Orders.Application.Mapping;
public static class OrderMappingData
{
    public static IEnumerable<OrderDto> ToOrderDtoList(this IEnumerable<Order> orders)
    {
        return orders.Select(o => new OrderDto( 
                Id: o.Id.Value,
                CustomerId: o.CustomerId.Value,
                RestaurantId: o.RestaurantId.Value,
                TrackingId: o.TrackingId.Value,
                VoucherId: o.VoucherId?.Value,
                TotalAmount: o.TotalAmount,
                OrderStatus: o.OrderStatus.ToString(),
                FailureMessages: o.FailureMessages,
                OrderItems: o.OrderItems.Select(oi => new OrderItemDto(
                    Id: oi.Id.Value,
                    OrderId: oi.OrderId.Value,
                    ProductId: oi.ProductId.Value,
                    Quantity: oi.Quantity,
                    Price: oi.Price,
                    SubTotal: oi.SubTotal
                )).ToList()
            ));
    }

    public static OrderDto ToOrderDto(this Order order)
    {
        return new OrderDto(
            Id: order.Id.Value,
            CustomerId: order.CustomerId.Value,
            RestaurantId: order.RestaurantId.Value,
            TrackingId: order.TrackingId.Value,
            VoucherId: order.VoucherId?.Value,
            TotalAmount: order.TotalAmount,
            OrderStatus: order.OrderStatus.ToString(),
            FailureMessages: order.FailureMessages,
            OrderItems: order.OrderItems.Select(oi => new OrderItemDto(
                Id: oi.Id.Value,
                OrderId: oi.OrderId.Value,
                ProductId: oi.ProductId.Value,
                Quantity: oi.Quantity,
                Price: oi.Price,
                SubTotal: oi.SubTotal
            )).ToList()
        );
    }

    
}
