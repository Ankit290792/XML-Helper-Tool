using Deloitte.Ink.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;

namespace Deloitte.Ink.Transition.Importer.Reports
{
    public static class SerilogExtension
    {

        public static IHostBuilder UseInkSerilog(this IHostBuilder builder, Action<HostBuilderContext, LoggerConfiguration> configureLogger, bool preserveStaticLogger = false)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (configureLogger == null)
                throw new ArgumentNullException(nameof(configureLogger));

            Exception exception = null;
            string omsLogMessage = "";

            builder.ConfigureServices((Action<HostBuilderContext, IServiceCollection>)((context, collection) =>
            {
                LoggerConfiguration loggerConfiguration = new LoggerConfiguration();

                try
                {
                    if (Convert.ToBoolean(context.Configuration.GetSection("OmsSettings:Include").Value))
                    {
                    }
                    else
                    {
                        omsLogMessage = "This deployment not configured for OMS.  See file logs instead.";
                    }
                }
                catch (Exception e)
                {
                    omsLogMessage = "Logger Configuration for Oms failed.";
                    exception = e;
                }

                configureLogger(context, loggerConfiguration);
                var logger = loggerConfiguration.CreateLogger();

                if (exception != null)
                    logger.Error(exception, omsLogMessage);
                else
                    logger.Information(omsLogMessage);


                if (preserveStaticLogger)
                {
                    collection.AddSingleton<ILoggerFactory>((Func<IServiceProvider, ILoggerFactory>)(services => (ILoggerFactory)new Serilog.Extensions.Logging.SerilogLoggerFactory((Serilog.ILogger)logger, true)));
                }
                else
                {
                    Log.Logger = (Serilog.ILogger)logger;
                    collection.AddSingleton<ILoggerFactory>((Func<IServiceProvider, ILoggerFactory>)(services => (ILoggerFactory)new Serilog.Extensions.Logging.SerilogLoggerFactory((Serilog.ILogger)null, true)));
                }
            }));

            return builder;
        }

        private static LogEventLevel GetLogLevel(IConfiguration configuration)
        {
            var logLevel = configuration.GetSection("Serilog:MinimumLevel:Default").Value;

            switch (logLevel)
            {
                case "Information":
                    return LogEventLevel.Information;
                case "Debug":
                    return LogEventLevel.Debug;
                case "Error":
                    return LogEventLevel.Error;
                case "Fatal":
                    return LogEventLevel.Fatal;
                case "Verbose":
                    return LogEventLevel.Verbose;
                case "Warning":
                    return LogEventLevel.Warning;

                default:
                    return LogEventLevel.Information;

            }
        }
    }
}
