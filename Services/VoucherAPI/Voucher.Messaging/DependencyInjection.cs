using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Voucher.Messaging.Consumers.CommandConsumers;
using Voucher.Messaging.Consumers.QueryConsumers;

namespace Voucher.Messaging;

public static class DependencyInjection
{
    public static IServiceCollection AddVoucherMessaging(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddMassTransit(x =>
        {
            // COMMAND SIDE (WriteDB)
            x.AddConsumer<OrderPlacedConsumer>();

            // QUERY SIDE (ReadDB)
            x.AddConsumer<VoucherCreatedConsumer>();
            x.AddConsumer<VoucherStatusUpdatedConsumer>();
            x.AddConsumer<VoucherUsageIncreasedConsumer>();

            // RabbitMQ Config
            x.UsingRabbitMq((context, rabbit) =>
            {
                rabbit.Host(cfg["RabbitMQ:Host"] ?? "localhost", h =>
                {
                    h.Username(cfg["RabbitMQ:Username"] ?? "guest");
                    h.Password(cfg["RabbitMQ:Password"] ?? "guest");
                });

                // COMMAND QUEUE
                rabbit.ReceiveEndpoint("order-placed-voucher-update", e =>
                {
                    e.ConfigureConsumer<OrderPlacedConsumer>(context);
                });

                // QUERY PROJECTION QUEUES
                rabbit.ReceiveEndpoint("voucher-created-queue", e =>
                {
                    e.ConfigureConsumer<VoucherCreatedConsumer>(context);
                });

                rabbit.ReceiveEndpoint("voucher-statusupdated-queue", e =>
                {
                    e.ConfigureConsumer<VoucherStatusUpdatedConsumer>(context);
                });

                rabbit.ReceiveEndpoint("voucher-usageincreased-queue", e =>
                {
                    e.ConfigureConsumer<VoucherUsageIncreasedConsumer>(context);
                });
            });
        });

        return services;
    }
}
