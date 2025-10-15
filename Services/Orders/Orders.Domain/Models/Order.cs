using Orders.Domain.Abstractions;
using Orders.Domain.Enums;
using Orders.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Domain.Models;
public class Order : Aggregate<OrderId>
{
    public CustomerId CustomerId { get; set; }

    public RestaurantId RestaurantId { get; set; }

    public TrackingId TrackingId { get; set; }

    public VoucherId? VoucherId { get; set; }

    public decimal TotalAmount => _orderItems.Sum(x => x.SubTotal);
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;

    public string? FailureMessages { get; set; }


    private readonly List<OrderItem> _orderItems = new();
    public IReadOnlyList<OrderItem> OrderItems => _orderItems.AsReadOnly();




    public static Order CreateOrder(
        Guid customerId,
        Guid restaurantId,
        Guid trackingId,
        Guid? voucherId
    )
       
    {
        var order = new Order
        {
            Id = OrderId.Of(Guid.NewGuid()),
            CustomerId = CustomerId.Of(customerId),
            RestaurantId = RestaurantId.Of(restaurantId),
            TrackingId = TrackingId.Of(trackingId),
            VoucherId = voucherId.HasValue ? VoucherId.Of(voucherId.Value) : null,
            OrderStatus = OrderStatus.Pending
        };
       
        return order;
    }

    public void AddOrderItem(ProductId productId, decimal price, int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);

        var orderItem = new OrderItem(Id, productId, price, quantity);
        _orderItems.Add(orderItem);
    }

    public void UpdateOrderStatus(OrderStatus status)
    {
        OrderStatus = status;
    }


}
