using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orders.Application.Data;
using Orders.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices
        (this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database") ?? configuration["ConnectionStrings:Database"];

        // Add services to the container.
       

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

        return services;
    }
}
