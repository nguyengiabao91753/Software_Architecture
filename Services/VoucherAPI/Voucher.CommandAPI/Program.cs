using Carter;
using Voucher.Application;
using Voucher.Infrastructure.Data.Extensions;
using Voucher.CommandAPI;
using Voucher.Messaging.Command;

var builder = WebApplication.CreateBuilder(args);

//   Fallback connection string
var connectionString =
    builder.Configuration.GetConnectionString("Database")
    ?? builder.Configuration["ConnectionStrings:DefaultConnection"];

// Inject fallback BACK INTO configuration
builder.Configuration["ConnectionStrings:Database"] = connectionString;

//   SERVICES
builder.Services
    .AddApplicationCommandServices(builder.Configuration)
    .AddInfrastructureWrite(builder.Configuration)
    .AddCommandApiServices(builder.Configuration);

builder.Services.AddVoucherCommandMessaging(builder.Configuration);

var app = builder.Build();

app.UseCommandApiServices();
await app.InitialiseWriteDbAsync();

app.Run();
