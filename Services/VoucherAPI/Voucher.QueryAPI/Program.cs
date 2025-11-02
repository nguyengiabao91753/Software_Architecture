using Microsoft.EntityFrameworkCore;
using Voucher.QueryAPI;
using Voucher.Application;
using Voucher.Infrastructure;
using Voucher.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Chạy QueryAPI ở port 5003
builder.WebHost.UseUrls("http://localhost:5003");

// Đăng ký DbContext kết nối tới ReadDB
builder.Services.AddDbContext<VoucherReadDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("QueryDb")));

// Gọi các tầng dịch vụ
builder.Services
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddQueryApiServices(builder.Configuration); // MassTransit, Carter, HealthCheck

var app = builder.Build();

// Kích hoạt middleware & route
app.UseQueryApiServices();

app.Run();
