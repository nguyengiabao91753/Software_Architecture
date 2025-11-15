using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Integrations.Consul.Extension;
public static class ConsulExtensions
{
    public static IApplicationBuilder RegisterWithConsul(this IApplicationBuilder app, IConfiguration configuration)
    {
        var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

        var consulConfig = configuration.GetSection("ConsulConfig");
        var consulAddress = consulConfig["Address"];
        var serviceName = consulConfig["ServiceName"];
        var servicePort = int.Parse(consulConfig["ServicePort"] ?? "80");
        var healthCheck = consulConfig["HealthCheck"] ?? "/health";

        var consulClient = new ConsulClient(config =>
        {
            config.Address = new Uri(consulAddress);
        });

        var hostName = Environment.GetEnvironmentVariable("HOSTNAME") ?? Dns.GetHostName();

        var registration = new AgentServiceRegistration
        {
            ID = $"{serviceName}-{Guid.NewGuid()}",
            Name = serviceName,
            Address = serviceName, // dùng service name trong docker-compose.yml
            Port = servicePort,
            Check = new AgentServiceCheck
            {
                HTTP = $"http://{serviceName}:{servicePort}{healthCheck}",
                Interval = TimeSpan.FromSeconds(10),
                Timeout = TimeSpan.FromSeconds(5),
                DeregisterCriticalServiceAfter = TimeSpan.FromHours(1)
            }
        };



        
        // Đăng ký khi app start
        lifetime.ApplicationStarted.Register(() =>
        {
            try
            {
                consulClient.Agent.ServiceRegister(registration).Wait();
                Console.WriteLine($"✅ Registered {serviceName} with Consul at {consulAddress}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to register {serviceName} with Consul: {ex.Message}");
            }
        });

        // Hủy đăng ký khi app stop
        lifetime.ApplicationStopped.Register(() =>
        {
            try
            {
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
                Console.WriteLine($"🧹 Deregistered {serviceName} from Consul");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Failed to deregister {serviceName}: {ex.Message}");
            }
        });

        return app;
    }

}
