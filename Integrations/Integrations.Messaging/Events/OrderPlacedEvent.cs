using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrations.Messaging.Events;
public record OrderPlacedEvent : IntegrationEvent
{
    public Guid OrderId { get; set; } = default!;
    public Guid CustomerId { get; set; } = default!;
    public Guid RestaurantId { get; set; } = default!;
    public Guid TrackingId { get; set; } = default!;
    public Guid? VoucherId { get; set; } = null;

    public decimal TotalAmount { get; set; } = default!;
    public string OrderStatus { get; set; } = string.Empty;
    public string? FailureMessages { get; set; } = default!;

    public List<OrderItemEvent> OrderItems { get; set; } = new();
}

public record OrderItemEvent
{
    public Guid ProductId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal SubTotal { get; set; }
}