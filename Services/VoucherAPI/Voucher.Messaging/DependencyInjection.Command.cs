using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Voucher.Messaging.Consumers.CommandConsumers;

namespace Voucher.Messaging.Command;

public static class CommandMessaging
{
    public static IServiceCollection AddVoucherCommandMessaging(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderPlacedConsumer>();

            x.SetEndpointNameFormatter(new DefaultEndpointNameFormatter(false));

            x.UsingRabbitMq((context, rabbit) =>
            {
                rabbit.Host(cfg["RabbitMQ:Host"] ?? "localhost", h =>
                {
                    h.Username(cfg["RabbitMQ:Username"] ?? "guest");
                    h.Password(cfg["RabbitMQ:Password"] ?? "guest");
                });

                rabbit.ReceiveEndpoint("order-placed-voucher-update", e =>
                {
                    e.ConfigureConsumer<OrderPlacedConsumer>(context);
                });
            });
        });

        return services;
    }
}
