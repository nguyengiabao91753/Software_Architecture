using Carter;
using Voucher.Application;
using Voucher.Infrastructure.Data.Extensions;
using Voucher.CommandAPI;
using MassTransit;
using Voucher.Messaging.Consumers;
using Integrations.Consul.Extension;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructureWriteServices(builder.Configuration)
    .AddCommandApiServices(builder.Configuration);

// MASS TRANSIT
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderPlacedConsumer>();

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

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseCommandApiServices();
await app.InitialiseWriteDatabaseAsync();


//Đăng ký consul
app.MapHealthChecks("/health");
app.RegisterWithConsul(builder.Configuration);

app.Run();
