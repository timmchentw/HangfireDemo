﻿using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Email;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace HangfireDemo.Shared
{
    public static class Extensions
    {
        public static IHostBuilder ConfigurSharedAppConfiguration(this IHostBuilder builder)
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

                string jobNamespace = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
                var env = hostContext.HostingEnvironment;
                var emailSinkInfo = new EmailConnectionInfo() { /*Assign Properties...*/ };

                loggerConfig.MinimumLevel.Information()
                .Enrich.WithProperty("Application", loggerInfo.ProgramNamespace)
                .Enrich.WithProperty("Environment", hostContext.HostingEnvironment.EnvironmentName)
                .WriteTo.Conditional(
                    condition => !env.IsDevelopment(),
                    writeTo => writeTo.Console()
                               .WriteTo.ApplicationInsights(new TelemetryConfiguration(loggerInfo.AppInsightsIntrumentalKey), TelemetryConverter.Traces)
                               .WriteTo.Email(loggerInfo.EmailSinkInfo, restrictedToMinimumLevel: LogEventLevel.Error));
            });
        }
    }
}
