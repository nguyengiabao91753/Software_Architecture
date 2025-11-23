using Microsoft.EntityFrameworkCore;
using Voucher.QueryAPI;
using Voucher.Application;
using Voucher.Infrastructure.Data;
using Voucher.Infrastructure.Data.Extensions;
using Voucher.Messaging.Query;
using Integrations.Consul.Extension;
using Integrations.OpenTelemetry.Extension;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Fallback connection string
var connectionString =
    builder.Configuration.GetConnectionString("Database")
    ?? builder.Configuration["ConnectionStrings:DefaultConnection"];

builder.Configuration["ConnectionStrings:Database"] = connectionString;

// Add Services 
builder.Services.AddInfrastructureRead(builder.Configuration);
builder.Services.AddVoucherQueryMessaging(builder.Configuration);
builder.Services.AddApplicationQueryServices(builder.Configuration);
builder.Services.AddQueryApiServices(builder.Configuration);

builder.Services.AddHealthChecks();

builder.Services.AddCustomOpenTelemetry("Voucher.QueryAPI");

var app = builder.Build();

// Prometheus Middleware
app.UseMetricServer();   
app.UseHttpMetrics();  

// API + Init Database
app.UseQueryApiServices();
await app.InitialiseReadDbAsync();

// Health Check + Consul
app.MapHealthChecks("/health");
app.MapMetrics("/metrics");
app.RegisterWithConsul(builder.Configuration);

app.Run();
