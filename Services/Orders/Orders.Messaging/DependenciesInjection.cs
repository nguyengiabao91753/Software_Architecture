using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Orders.Messaging.Consumers;
using Orders.Messaging.Interfaces;
using Orders.Messaging.Publishers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Messaging;
public static class DependenciesInjection
{
    public static IServiceCollection AddOrderMessaging(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderEventConsumer>();
            // Other consumers can be added here
        });

        return services;
    }
}
