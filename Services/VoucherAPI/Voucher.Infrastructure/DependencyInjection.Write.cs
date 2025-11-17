using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Voucher.Application.Abstractions;
using Voucher.Infrastructure.Data;
using Voucher.Infrastructure.Repositories;

namespace Voucher.Infrastructure.Data.Extensions;

public static class WriteInfrastructure
{
    public static IServiceCollection AddInfrastructureWrite(
        this IServiceCollection services, IConfiguration cfg)
    {
        var connection = cfg.GetConnectionString("Database")?? cfg.GetConnectionString("CommandDb");

        services.AddDbContext<WriteDbContext>(options =>
            options.UseSqlServer(connection));

        services.AddScoped<IVoucherRepository, VoucherRepository>();

        return services;
    }
}
