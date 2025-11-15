using Carter;
using HealthChecks.UI.Client;
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
