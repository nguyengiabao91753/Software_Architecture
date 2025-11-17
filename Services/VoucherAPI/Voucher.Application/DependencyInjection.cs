using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Voucher.Application;

public static class DependencyInjection
{
    // ONLY COMMAND HANDLERS
    public static IServiceCollection AddApplicationCommandServices(
        this IServiceCollection services,
        IConfiguration cfg)
    {
        services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        // Remove query handlers
        FilterHandlers(services, "Voucher.Application.Commands");

        return services;
    }

    // ONLY QUERY HANDLERS
    public static IServiceCollection AddApplicationQueryServices(
        this IServiceCollection services,
        IConfiguration cfg)
    {
        services.AddMediatR(opt =>
        {
            opt.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        // Remove command handlers
        FilterHandlers(services, "Voucher.Application.Queries");

        return services;
    }

    // Remove all handlers that are not in correct namespace
    private static void FilterHandlers(IServiceCollection services, string correctNamespace)
    {
        var handlers = services
            .Where(s =>
                s.ServiceType.IsGenericType &&
                (
                    s.ServiceType.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                    s.ServiceType.GetGenericTypeDefinition() == typeof(IRequestHandler<>)
                ))
            .Where(s =>
                s.ImplementationType?.Namespace?.StartsWith(correctNamespace) == false)
            .ToList();

        foreach (var h in handlers)
            services.Remove(h);
    }
}
