using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Voucher.Application.Abstractions;
using Voucher.Infrastructure.Data;
using Voucher.Infrastructure.Repositories;

namespace Voucher.Infrastructure.Data.Extensions;

public static class ReadInfrastructure
{
    public static IServiceCollection AddInfrastructureRead(
        this IServiceCollection services, IConfiguration cfg)
    {
        var connection = cfg.GetConnectionString("QueryDb");

        services.AddDbContext<VoucherReadDbContext>(options =>
            options.UseSqlServer(connection));

        services.AddScoped<IQueryRepository, QueryRepository>();

        return services;
    }
}
