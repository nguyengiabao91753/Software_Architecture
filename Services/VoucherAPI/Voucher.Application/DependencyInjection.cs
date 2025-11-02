using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Voucher.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration cfg)
    {
        // ✅ Cú pháp mới của MediatR (v12+)
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Nếu có FluentValidation hoặc Mapster, bạn có thể add thêm ở đây
        // services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        // TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

        return services;
    }
}
