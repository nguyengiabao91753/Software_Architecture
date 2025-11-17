using Consul;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using Microsoft.Extensions.DependencyInjection;
using Integrations.Messaging.HealthCheck;
using HealthCheck.Application.Helpers;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Add Razor Pages + HttpClient + Static files
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Health checks (RabbitMQ + custom consul services check)
builder.Services.AddHealthChecks()
    .AddRabbitMQ(
        rabbitConnectionString: builder.Configuration["MessageBroker:HostAddress"] ?? " ",
        name: builder.Configuration["MessageBroker:HostName"] ?? "rabbitmq",
        timeout: TimeSpan.FromSeconds(5),
        tags: new[] { "ready", "rabbit" }
    )
    .AddCheck("consul-services", new FuncHealthCheck(async ct =>
    {
        try
        {
            using var consul = new ConsulClient(c => c.Address = new Uri(builder.Configuration["ConsulConfig:Address"]));
            var catalog = await consul.Catalog.Services(ct);
            var services = new List<object>();

            foreach (var serviceName in catalog.Response.Keys)
            {
                var health = await consul.Health.Service(serviceName, "", false, ct);
                var grouped = health.Response
                    .GroupBy(h => h.Service.ID)
                    .Select(g => new {
                        ServiceId = g.Key,
                        ServiceName = serviceName,
                        Status = g.SelectMany(x => x.Checks).Select(c => c.Status.ToString()).Distinct().ToList()
                    });

                services.AddRange(grouped);
            }

            // Healthy + data: health UI will include this in /health-json
            return HealthCheckResult.Healthy("Consul services health", data: new Dictionary<string, object> { ["services"] = services });
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(ex.Message);
        }
    }));

// HealthChecks UI
builder.Services.AddHeatlthCheckUIConfig(builder.Configuration);


// optional: small helper to serve static assets
builder.Services.AddDirectoryBrowser();


var app = builder.Build();

// Static files (for JS/CSS)
app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();
app.MapControllers();

// Health endpoints
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

// Add an endpoint to expose Consul raw services info (for the custom dashboard)
app.MapGet("/consul-services", async (IConfiguration config) =>
{
    var consulAddress = config["ConsulConfig:Address"];
    using var consul = new ConsulClient(c => c.Address = new Uri(consulAddress));
    var catalog = await consul.Catalog.Services();
    var resultList = new List<object>();

    foreach (var service in catalog.Response.Keys)
    {
        var health = await consul.Health.Service(service, "", false);
        resultList.Add(new
        {
            service,
            nodes = health.Response.Select(s => new
            {
                s.Service.ID,
                s.Service.Service,
                s.Service.Address,
                s.Service.Port,
                Checks = s.Checks.Select(c => new
                {
                    c.CheckID,
                    c.Name,
                    Status = c.Status.ToString(),
                    c.Output
                })
            })
        });
    }

    return Results.Ok(resultList);
});

app.MapGet("/", () => Results.Redirect("/dashboard"));

// Map Razor dashboard page (we will create it)
app.MapRazorPages();



app.Run();
