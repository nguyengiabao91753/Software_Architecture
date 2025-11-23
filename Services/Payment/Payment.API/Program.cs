using Prometheus;
using Integrations.Consul.Extension;
using Integrations.Messaging.Events;
using Integrations.Messaging.Masstransit;
using Payment.Messaging;
using Integrations.Consul.Extension;
using Integrations.OpenTelemetry.Extension;
using Payment.Messaging.Consumer;

var builder = WebApplication.CreateBuilder(args);

// Monitoring
builder.Services.AddControllers();

// Add MassTransit
builder.Services.AddPaymentMessaging(builder.Configuration);
//builder.Services.AddMessageBroker(builder.Configuration, new[]
//{
//    typeof(OrderPlacedConsumer).Assembly,
//    typeof(OrderPlacedEvent).Assembly       // ← cũng thêm vào để biết publish topology
//});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.AddCustomOpenTelemetry("Payment.API");
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Prometheus middleware
app.UseMetricServer();
app.UseHttpMetrics();

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

// Consul
app.RegisterWithConsul(builder.Configuration);

app.Run();
