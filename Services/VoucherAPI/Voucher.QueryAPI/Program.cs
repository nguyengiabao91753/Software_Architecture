using Microsoft.EntityFrameworkCore;
using Voucher.QueryAPI;
using Voucher.Application;
using Voucher.Infrastructure.Data;
using Voucher.Infrastructure.Data.Extensions;
using Voucher.Messaging.Query;
using Integrations.Consul.Extension;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5003");

// =========================
//   Fallback connection string
// =========================
var connectionString =
    builder.Configuration.GetConnectionString("Database")
    ?? builder.Configuration["ConnectionStrings:DefaultConnection"];

// Inject fallback back into configuration
builder.Configuration["ConnectionStrings:Database"] = connectionString;

// =========================
//   Read DB
// =========================
builder.Services.AddInfrastructureRead(builder.Configuration);   // không đổi

builder.Services.AddVoucherQueryMessaging(builder.Configuration);
builder.Services.AddApplicationQueryServices(builder.Configuration);
builder.Services.AddQueryApiServices(builder.Configuration);
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseQueryApiServices();
await app.InitialiseReadDbAsync();

app.MapHealthChecks("/health");
app.RegisterWithConsul(builder.Configuration);

app.Run();
