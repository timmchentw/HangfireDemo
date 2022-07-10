using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Email;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;

namespace HangfireDemo.Shared
{
    public static class Extensions
    {
        public static IHostBuilder ConfigureSharedAppConfiguration(this IHostBuilder builder)
        {
            return builder.ConfigureAppConfiguration((hostContext, config) =>
             {
                 var env = hostContext.HostingEnvironment;
                 string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                 config = config.SetBasePath(assemblyDirectory)
                                 .AddJsonFile("sharedsettings.json", optional: false, reloadOnChange: true)
                                 .AddJsonFile($"sharedsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                                 .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
                                 .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
             });
        }

        public static IHostBuilder ConfigureSerilog(this IHostBuilder builder, Func<HostBuilderContext, LoggerInfo> getLoggerInfo)
        {
            return builder.UseSerilog((hostContext, loggerConfig) =>
            {
                var loggerInfo = getLoggerInfo(hostContext);

                loggerConfig.MinimumLevel.Information()
                .Enrich.WithProperty("Application", loggerInfo.ProgramNamespace)
                .Enrich.WithProperty("Environment", hostContext.HostingEnvironment.EnvironmentName)
                .WriteTo.Console()
                .WriteTo.Conditional(
                    condition => loggerInfo.CanLog,
                    writeTo => writeTo.ApplicationInsights(new TelemetryConfiguration(loggerInfo.AppInsightsIntrumentalKey), TelemetryConverter.Traces)
                               .WriteTo.Email(loggerInfo.EmailSinkInfo, restrictedToMinimumLevel: LogEventLevel.Error));
            });
        }

        public static string GetDescription<T>(this T enumValue) where T : struct
        {
            FieldInfo fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            var attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return ((DescriptionAttribute)attributes[0]).Description;
            }
            else
            {
                return enumValue.ToString();
            };
        }
    }
}
