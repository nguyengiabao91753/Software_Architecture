using Integrations.Messaging.Events;
using MassTransit;
using Orders.Messaging.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Messaging.Publishers;
public class OrderEventPublisher : IOrderEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public OrderEventPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }
    public async Task PublishOrderPlacedAsync(OrderPlacedEvent model)
    {
        await _publishEndpoint.Publish(model);

    }
}
