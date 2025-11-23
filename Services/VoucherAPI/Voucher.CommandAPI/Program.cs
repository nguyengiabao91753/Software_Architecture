using Carter;
using Voucher.Application;
using Voucher.Infrastructure.Data.Extensions;
using Voucher.CommandAPI;
using Voucher.Messaging.Command;
using Integrations.Consul.Extension;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Fallback connection string
var connectionString =
    builder.Configuration.GetConnectionString("Database")
    ?? builder.Configuration["ConnectionStrings:DefaultConnection"];

builder.Configuration["ConnectionStrings:Database"] = connectionString;

// Services
builder.Services
    .AddApplicationCommandServices(builder.Configuration)
    .AddInfrastructureWrite(builder.Configuration)
    .AddCommandApiServices(builder.Configuration);

builder.Services.AddVoucherCommandMessaging(builder.Configuration);

var app = builder.Build();

// ⭐ Prometheus
app.UseMetricServer();    // expose metrics server (default /metrics)
app.UseHttpMetrics();     // collect HTTP metrics

// Carter
app.UseCommandApiServices();

// Init database
await app.InitialiseWriteDbAsync();

// Healthcheck
app.MapHealthChecks("/health");

// ⭐ Nếu muốn chắc chắn 100% không lỗi routing:
app.MapMetrics("/metrics");

// Consul
app.RegisterWithConsul(builder.Configuration);

app.Run();
