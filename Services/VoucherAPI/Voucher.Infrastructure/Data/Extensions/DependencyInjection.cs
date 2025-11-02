using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Voucher.Application.Abstractions;
using Voucher.Infrastructure.Data;
using Voucher.Infrastructure.Repositories;

namespace Voucher.Infrastructure.Data.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration cfg)
    {
        // Write DB cho CommandAPI
        services.AddDbContext<WriteDbContext>(options =>
            options.UseSqlServer(cfg.GetConnectionString("CommandDb")));

        // Read DB cho QueryAPI
        services.AddDbContext<VoucherReadDbContext>(options =>
            options.UseSqlServer(cfg.GetConnectionString("QueryDb")));

        // Repository
        services.AddScoped<IVoucherRepository, VoucherRepository>();

        return services;
    }
}
