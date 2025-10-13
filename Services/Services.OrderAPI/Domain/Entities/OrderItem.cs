using System;
using System.Collections.Generic;

namespace Services.OrderAPI.Domain.Entities;

public partial class OrderItem
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public Guid ProductId { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public decimal SubTotal { get; set; }

    public virtual Order Order { get; set; } = null!;
}
