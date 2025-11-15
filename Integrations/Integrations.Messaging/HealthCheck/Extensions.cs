using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrations.Messaging.HealthCheck;
public static class Extensions
{
    public static IServiceCollection AddHealhCheckConfig(this IServiceCollection services, IConfiguration configuration)
    {
       services.AddHealthChecks().AddRabbitMQ(
        rabbitConnectionString: configuration["MessageBroker:HostAddress"] ??" ",
        name: configuration["MessageBroker:HostName"] ?? " ",
        timeout: TimeSpan.FromSeconds(5),
        tags: new[] { "ready", "rabbit" }
        );



        return services;
    }
}
