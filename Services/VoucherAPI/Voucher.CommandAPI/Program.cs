using Carter;
using Voucher.Application;
using Voucher.Infrastructure.Data.Extensions; // dùng để gọi AddInfrastructureServices
using Voucher.CommandAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddCommandApiServices(builder.Configuration);

var app = builder.Build();

app.UseCommandApiServices();
await app.InitialiseDatabaseAsync();

app.Run();
