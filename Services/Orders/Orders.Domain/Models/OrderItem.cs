using Orders.Domain.Abstractions;
using Orders.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Domain.Models;
public class OrderItem : Entity<OrderItemId>
{
    public OrderItem(OrderId orderId, ProductId productId, decimal price, int quantity)
    {
        Id = OrderItemId.Of(Guid.NewGuid());
        OrderId = orderId;
        ProductId = productId;
        Price = price;
        Quantity = quantity;
        
    }

    public OrderId OrderId { get; set; }

    public ProductId ProductId { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public decimal SubTotal => Price * Quantity;

   
}
