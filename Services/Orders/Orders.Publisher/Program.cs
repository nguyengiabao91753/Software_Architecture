using Orders.Messaging;
using Integrations.Messaging.Masstransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Integrations.Messaging.HealthCheck;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();







// Add services to the container.
builder.Services.AddOrderMessaging();

//Add Masstransit Message Broker
builder.Services.AddMessageBroker(builder.Configuration, typeof(Program).Assembly);






//Add Health Check
builder.Services.AddHealhCheckConfig(builder.Configuration)
                .AddHeatlthCheckUIConfig(builder.Configuration);


var app = builder.Build();

app.MapGet("/", context =>
{
    context.Response.Redirect("/health-ui");
    return Task.CompletedTask;
});



// --- Endpoint Health Check ---
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


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
