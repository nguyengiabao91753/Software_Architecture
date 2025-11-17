// Services/VoucherAPI/Voucher.CommandAPI/DependencyInjection.cs

using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Voucher.CommandAPI;

public static class DependencyInjection
{
    public static IServiceCollection AddCommandApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        services.AddCarter();

        return services;
    }

    public static WebApplication UseCommandApiServices(this WebApplication app)
    {
        app.MapCarter();

        return app;
    }
}
