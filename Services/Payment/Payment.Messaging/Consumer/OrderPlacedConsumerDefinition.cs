using Integrations.Messaging.Events;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Messaging.Consumer;
public class OrderPlacedConsumerDefinition : ConsumerDefinition<OrderPlacedConsumer>
{
    public OrderPlacedConsumerDefinition()
    {
        EndpointName = "payment-order-queue";
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<OrderPlacedConsumer> consumerConfigurator)
    {
      

        //if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rmq)
        //{
        //    rmq.Bind<OrderPlacedEvent>(s =>
        //    {
                
        //        s.ExchangeType = "fanout";
        //    });
        //}

        
    }
}
