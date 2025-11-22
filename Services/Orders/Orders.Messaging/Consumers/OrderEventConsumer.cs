using Integrations.Messaging.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orders.Application.Data;
using Orders.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Messaging.Consumers;
public class OrderEventConsumer : IConsumer<OrderCancelled>
{
    private readonly IApplicationDbContext _db;

    public OrderEventConsumer(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Consume(ConsumeContext<OrderCancelled> context)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == OrderId.Of(context.Message.OrderId));
        if (order != null)
        {
            order.UpdateOrderStatus("Cancelled");
            await _db.SaveChangesAsync();
        }
    }
}
