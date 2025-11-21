using Prometheus;
using Integrations.Messaging.Masstransit;
using Payment.Messaging;
using Integrations.Consul.Extension;

var builder = WebApplication.CreateBuilder(args);

// Monitoring
builder.Services.AddControllers();

// Add MassTransit
builder.Services.AddPaymentMessaging(builder.Configuration);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

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
