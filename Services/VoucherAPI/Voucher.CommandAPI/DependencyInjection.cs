using Carter;
using HealthChecks.UI.Client;
using MassTransit; // ✅ thêm thư viện MassTransit
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Voucher.CommandAPI;

public static class DependencyInjection
{
    public static IServiceCollection AddCommandApiServices(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddCarter();

        // ✅ Thêm MassTransit (RabbitMQ publisher)
        services.AddMassTransit(x =>
        {
            // Nếu sau này có Consumer riêng cho CommandAPI, có thể AddConsumer tại đây
            x.UsingRabbitMq((context, rabbit) =>
            {
                rabbit.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                // (Tuỳ chọn) đặt tên instance endpoint mặc định
                rabbit.ConfigureEndpoints(context);
            });
        });

        // ✅ HealthCheck cho WriteDB
        services.AddHealthChecks()
            .AddSqlServer(cfg.GetConnectionString("CommandDb")!);

        return services;
    }

    public static WebApplication UseCommandApiServices(this WebApplication app)
    {
        app.MapCarter();

        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        return app;
    }
}
