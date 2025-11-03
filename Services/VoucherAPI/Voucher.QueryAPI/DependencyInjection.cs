using Carter;
using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Voucher.QueryAPI.Consumers;

namespace Voucher.QueryAPI;

public static class DependencyInjection
{
    public static IServiceCollection AddQueryApiServices(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddCarter();

        // ================================
        // 🐇 Cấu hình MassTransit + RabbitMQ
        // ================================
        services.AddMassTransit(x =>
        {
            // 1️⃣ Đăng ký tất cả các Consumer
            x.AddConsumer<VoucherCreatedConsumer>();
            x.AddConsumer<VoucherUsageIncreasedConsumer>();
            x.AddConsumer<VoucherStatusUpdatedConsumer>();

            // 2️⃣ Cấu hình kết nối RabbitMQ
            x.UsingRabbitMq((context, rabbit) =>
            {
                rabbit.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                // 3️⃣ Khai báo các queue riêng biệt
                rabbit.ReceiveEndpoint("voucher-created-queue", e =>
                {
                    e.ConfigureConsumer<VoucherCreatedConsumer>(context);
                });

                rabbit.ReceiveEndpoint("voucher-usageincreased-queue", e =>
                {
                    e.ConfigureConsumer<VoucherUsageIncreasedConsumer>(context);
                });

                rabbit.ReceiveEndpoint("voucher-statusupdated-queue", e =>
                {
                    e.ConfigureConsumer<VoucherStatusUpdatedConsumer>(context);
                });
            });
        });

        // ================================
        // 🧠 Healthcheck cho ReadDB
        // ================================
        services.AddHealthChecks()
            .AddSqlServer(cfg.GetConnectionString("QueryDb")!);

        return services;
    }

    public static WebApplication UseQueryApiServices(this WebApplication app)
    {
        app.MapCarter();

        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        return app;
    }
}
