using Microsoft.Extensions.DependencyInjection;
using Orders.Application.IServices;
using Services.OrderAPI.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Application;
public static class DependenciesInjection
{
    public static IServiceCollection AddOrderServices
       (this IServiceCollection services)
    {
        services.AddScoped<IOrderService, OrderService>();
        return services;
    }
}