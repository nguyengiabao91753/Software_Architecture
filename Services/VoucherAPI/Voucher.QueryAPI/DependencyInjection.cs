using Carter;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Voucher.QueryAPI;

public static class DependencyInjection
{
    public static IServiceCollection AddQueryApiServices(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddCarter();

        // Healthcheck cho ReadDB
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
