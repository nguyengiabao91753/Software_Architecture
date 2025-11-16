using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Voucher.Application.Abstractions;
using Voucher.Infrastructure.Data;
using Voucher.Infrastructure.Repositories;

namespace Voucher.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration cfg)
    {
        // WriteDbContext (Command)
        services.AddDbContext<WriteDbContext>(options =>
            options.UseSqlServer(cfg.GetConnectionString("CommandDb")));

        // Đổi sang VoucherReadDbContext (Query)
        services.AddDbContext<VoucherReadDbContext>(options =>
            options.UseSqlServer(cfg.GetConnectionString("QueryDb")));

        // Repository
        services.AddScoped<IVoucherRepository, VoucherRepository>();
        services.AddScoped<IQueryRepository, QueryRepository>();

        return services;
    }
}
