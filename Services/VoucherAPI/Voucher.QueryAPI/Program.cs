using Microsoft.EntityFrameworkCore;
using Voucher.QueryAPI;
using Voucher.Application;
using Voucher.Infrastructure.Data;
using Voucher.Infrastructure.Data.Extensions;
using Voucher.Application.Queries.GetVouchers;
using MediatR;
using Voucher.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5003");

// DB Read
builder.Services.AddDbContext<VoucherReadDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("QueryDb")));

builder.Services.AddVoucherMessaging(builder.Configuration);

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<GetVouchersHandler>();
});

builder.Services.AddQueryApiServices(builder.Configuration);

var app = builder.Build();

app.UseQueryApiServices();

await app.InitialiseReadDbAsync();

app.Run();
