using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.Extensions.Configuration;

namespace Integrations.Messaging.HealthCheck;
public static class HealthCheckUI
{
    public static IServiceCollection AddHeatlthCheckUIConfig(this  IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecksUI(setupSettings: setup =>
        {
            setup.SetEvaluationTimeInSeconds(10);
            setup.AddHealthCheckEndpoint("Microservices Health", "http://health_check:9000/health");
        }).AddInMemoryStorage();

        return services;
    }
}
