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
        //
        // 1) REGISTER WriteDbContext ONLY when CommandDb exists (CommandAPI side)
        //
        var commandDb = cfg.GetConnectionString("CommandDb");
        if (!string.IsNullOrWhiteSpace(commandDb))
        {
            services.AddDbContext<WriteDbContext>(options =>
                options.UseSqlServer(commandDb));
        }

        //
        // ⚠ 2) DO NOT REGISTER ReadDbContext HERE
        // QueryAPI registers its own ReadDbContext in Program.cs.
        //
        //   services.AddDbContext<VoucherReadDbContext>(...)  <-- REMOVE

        //
        // 3) Register Repository (shared by both Command and Query)
        // 
        services.AddScoped<IVoucherRepository, VoucherRepository>();

        return services;
    }
}
