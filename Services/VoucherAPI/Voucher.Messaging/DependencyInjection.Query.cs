using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Voucher.Messaging.Consumers.QueryConsumers;

namespace Voucher.Messaging.Query;

public static class QueryMessaging
{
    public static IServiceCollection AddVoucherQueryMessaging(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<VoucherCreatedConsumer>();
            x.AddConsumer<VoucherStatusUpdatedConsumer>();
            x.AddConsumer<VoucherUsageIncreasedConsumer>();

            x.SetEndpointNameFormatter(new DefaultEndpointNameFormatter(false));

            x.UsingRabbitMq((context, rabbit) =>
            {
                rabbit.Host(cfg["RabbitMQ:Host"] ?? "localhost", h =>
                {
                    h.Username(cfg["RabbitMQ:Username"] ?? "guest");
                    h.Password(cfg["RabbitMQ:Password"] ?? "guest");
                });

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
