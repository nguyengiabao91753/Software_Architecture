using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Messaging.Consumer;

namespace Payment.Messaging;

public static class DependenciesInjection
{
    public static IServiceCollection AddPaymentMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderPlacedConsumer, OrderPlacedConsumerDefinition>();
            x.UsingRabbitMq((context, configurator) =>
            {
                configurator.Host("localhost", "/", host =>
                {
                    host.Username(configuration["MessageBroker:UserName"]);
                    host.Password(configuration["MessageBroker:Password"]);
                });
                configurator.ConfigureEndpoints(context);
            });
        });


        return services;
    }
}
