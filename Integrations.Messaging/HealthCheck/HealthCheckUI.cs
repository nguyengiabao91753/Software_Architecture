using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

namespace Integrations.Messaging.HealthCheck;
public static class HealthCheckUI
{
    public static IServiceCollection AddHeatlthCheckUIConfig(this  IServiceCollection services)
    {
        services.AddHealthChecksUI(setupSettings: setup =>
        {
            setup.SetEvaluationTimeInSeconds(10); // 10s kiểm tra 1 lần
            setup.AddHealthCheckEndpoint("Publisher Health", "/health");
        }).AddInMemoryStorage();

        return services;
    }
}
