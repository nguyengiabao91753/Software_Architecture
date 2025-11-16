using Consul;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using Microsoft.Extensions.DependencyInjection;
using Integrations.Messaging.HealthCheck;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Cấu hình HealthCheck UI
builder.Services.AddHealthChecks()
    // Check RabbitMQ
    .AddRabbitMQ(
        rabbitConnectionString: builder.Configuration["MessageBroker:HostAddress"] ?? " ",
        name: builder.Configuration["MessageBroker:HostName"] ?? " ",
        timeout: TimeSpan.FromSeconds(5),
        tags: new[] { "ready", "rabbit" }
    )
    // Check Consul registered services
    .AddCheck("consul-services", () =>
    {
        try
        {
            using var consul = new ConsulClient(c => c.Address = new Uri(builder.Configuration["ConsulConfig:Address"]));
            var services = consul.Agent.Services().Result.Response;
            return services.Any()
                ? HealthCheckResult.Healthy($"{services.Count} services registered in Consul")
                : HealthCheckResult.Degraded("No services registered in Consul");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(ex.Message);
        }
    });

builder.Services.AddHeatlthCheckUIConfig(builder.Configuration);

var app = builder.Build();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";
    options.ApiPath = "/health-json";
});


app.Run();
