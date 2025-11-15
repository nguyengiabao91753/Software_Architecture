using Microsoft.Extensions.DependencyInjection;
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
        services.AddScoped<IOrderEventPublisher, OrderEventPublisher>();

        return services;
    }
}
