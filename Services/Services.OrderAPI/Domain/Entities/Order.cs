using System;
using System.Collections.Generic;

namespace Services.OrderAPI.Domain.Entities;

public partial class Order
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public Guid RestaurantId { get; set; }

    public Guid TrackingId { get; set; }

    public Guid? VoucherId { get; set; }

    public decimal Price { get; set; }

    public string OrderStatus { get; set; } = null!;

    public string? FailureMessages { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
