using Integrations.Consul.Extension;
using Integrations.Messaging.Events;
using Integrations.Messaging.Masstransit;
using Orders.Application;
using Orders.Infrastructure;
using Orders.Infrastructure.Extensions;
using Orders.Messaging;
using Shares.SystemConfig.Authentication;

using Prometheus; // Prometheus middleware

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddInfrastructureServices(builder.Configuration)
                .AddOrderMessaging()
                .AddOrderServices();

builder.Services.AddMessageBroker(builder.Configuration, typeof(Program).Assembly);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddAppAuthentication();
builder.AddSwaggerWithJWT();

builder.Services.AddHealthChecks();

var app = builder.Build();

// Bật middleware Prometheus
app.UseHttpMetrics();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await app.InitialiseDatabaseAsync();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");
app.RegisterWithConsul(builder.Configuration);

// Expose endpoint metrics
app.MapMetrics("/metrics"); // đường dẫn scrape cho Prometheus

app.Run();
