using Carter;
using Voucher.Application;
using Voucher.Infrastructure.Data.Extensions;
using Voucher.CommandAPI;
using MassTransit;
using Voucher.Messaging.Consumers.CommandConsumers;  // ⭐ SỬA ĐÚNG NAMESPACE

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationCommandServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddCommandApiServices(builder.Configuration);

// MASS TRANSIT
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderPlacedConsumer>();   // ⭐ Consumer OK

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"], h =>
        {
            h.Username(builder.Configuration["RabbitMQ:Username"]);
            h.Password(builder.Configuration["RabbitMQ:Password"]);
        });

        cfg.ReceiveEndpoint("order-placed-voucher-update", e =>
        {
            e.ConfigureConsumer<OrderPlacedConsumer>(context);
        });
    });
});

var app = builder.Build();

app.UseCommandApiServices();
await app.InitialiseWriteDbAsync();

app.Run();
