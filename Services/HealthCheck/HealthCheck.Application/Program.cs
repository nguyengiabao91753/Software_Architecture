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
    // Check Consul registered services with health
    .AddCheck("consul-services", () =>
    {
        try
        {
            using var consul = new ConsulClient(c =>
                c.Address = new Uri(builder.Configuration["ConsulConfig:Address"])
            );

            // Lấy toàn bộ service name
            var cat = consul.Catalog.Services().Result.Response;

            var serviceHealthList = new List<object>();

            foreach (var serviceName in cat.Keys)
            {
                // Get health for each service
                var health = consul.Health.Service(serviceName, "", passingOnly: false).Result.Response;

                // Lấy trạng thái unique theo Service ID
                var grouped = health
                    .GroupBy(h => h.Service.ID)
                    .Select(g => new
                    {
                        ServiceId = g.Key,
                        ServiceName = serviceName,
                        Status = g.SelectMany(x => x.Checks)
                                  .Select(c => c.Status.ToString())
                                  .Distinct()
                                  .ToList()
                    });

                serviceHealthList.AddRange(grouped);
            }

            return HealthCheckResult.Healthy("Consul services health", data: new Dictionary<string, object>
            {
                ["services"] = serviceHealthList
            });
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
