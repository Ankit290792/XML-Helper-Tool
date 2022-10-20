using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights;


namespace Deloitte.Ink.Transition.Importer.Reports.StartUp
{
    public static class ConfigureAppBuilder
    {
        public static IHostBuilder BuildAppHost(this HostBuilder builder)
        {
            builder
            .ConfigureServices((hostContext, services) =>
             {
                 services.RegisterServices(hostContext.Configuration);
             }).

            ConfigureLogging(logging =>
            {
                logging.ClearProviders(); //  clear logger providers here, configured on the next line
            })

           .UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));// using serilog 
           
            return builder;

        }
    }
}
