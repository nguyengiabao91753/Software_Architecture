using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Shares.SystemConfig.Dependencies;
public static class DependencyInjection
{
    public static void AddServiceSingleton(this IHostApplicationBuilder builder)
    {

    }

    public static void AddServiceScoped(this IHostApplicationBuilder builder)
    {
        

    }

    public static void AddServiceTransient(this IHostApplicationBuilder builder)
    {
    }

    public static void AddServiceHttpClient(this IHostApplicationBuilder builder)
    {
        //builder.Services.AddHttpClient("AuthAPI", u => u.BaseAddress = new Uri("https://localhost:7001/"));

    }
}
