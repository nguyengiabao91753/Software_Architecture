using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Integrations.OpenTelemetry.Extension
{
    public static class OpenTelemetryExtension
    {
        public static IServiceCollection AddCustomOpenTelemetry(
            this IServiceCollection services,
            string serviceName)
        {
            services.AddOpenTelemetry()
                .ConfigureResource(resource =>
                    resource.AddService(serviceName))
                .WithTracing(tracing =>
                {
                    tracing
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddSqlClientInstrumentation(o =>
                        {
                           o.SetDbStatementForText = true;
                        })
                        .AddSource("MassTransit")
                        .AddOtlpExporter(opt =>
                        {
                            // Jaeger đang chạy ở docker? 
                            // Nếu dùng Jaeger Collector thì giữ nguyên
                            opt.Endpoint = new Uri("http://otel-collector:4317");
                        });
                });

            return services;
        }
    }
}
