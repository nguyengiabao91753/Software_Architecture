using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Application.Dtos;
public record OrderDto
(
    Guid Id,
    Guid CustomerId,
    Guid RestaurantId,
    Guid TrackingId,
    Guid? VoucherId,
    decimal TotalAmount,
    string OrderStatus,
    string? FailureMessages,
    List<OrderItemDto> OrderItems
);
