using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Voucher.Application;

public static class DependencyInjection
{
    // COMMAND API → chỉ load Command handlers
    public static IServiceCollection AddApplicationCommandServices(
        this IServiceCollection services,
        IConfiguration cfg)
    {
        var commandAssembly = typeof(Commands.CreateVoucher.CreateVoucherCommand).Assembly;

        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssemblies(commandAssembly);
        });

        return services;
    }

    // QUERY API → chỉ load Query handlers
    public static IServiceCollection AddApplicationQueryServices(
        this IServiceCollection services,
        IConfiguration cfg)
    {
        var queryAssembly = typeof(Queries.GetVouchers.GetVouchersQuery).Assembly;

        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssemblies(queryAssembly);
        });

        return services;
    }
}
