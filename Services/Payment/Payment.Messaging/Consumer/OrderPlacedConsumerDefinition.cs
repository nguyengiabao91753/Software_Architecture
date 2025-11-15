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


        //Nếu dùng Fanount thì không cần cấu hình gỉ cả ở đây
        //Đây là nơi cấu hình cho các routing key, headers nếu dùng Direct, Topic, Headers exchange


    }
}
