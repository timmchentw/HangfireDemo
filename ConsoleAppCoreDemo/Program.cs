using HangfireDemo.Shared;
using HangfireDemo.Shared.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog.Sinks.Email;
using System;
using System.Net;
using System.Reflection;

namespace ConsoleAppCoreDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            string jobNamespace = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
            var startupValues = new StartupValues() { ProgramNamespace = jobNamespace };
            var getLoggerInfo = LoggerInfoAccessor.GetInfo(jobNamespace);

            JobHost.CreateJobHostBuilder(args, startupValues, getLoggerInfo)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<JobService>();    // Register job service

                    // For JobService with Application Insights
                    if (false)
                    {
                        var appInsightsConfig = hostContext.Configuration.GetSection(nameof(HangfireDemo.Shared.Config.ApplicationInsights))
                            .Get<HangfireDemo.Shared.Config.ApplicationInsights>();
                        services.AddApplicationInsightsTelemetryWorkerService(appInsightsConfig.InstrumentalKey);
                        services.AddHostedService<JobSeriveBaseWithTelemetry>();
                    }

                })
                .UseConsoleLifetime()   // For flush all resources before stopping the app (e.g. App Insights)
                .Build()
                .Run();    // Run logic in job service
                           // (If you want to throw exceptions in hosted service,
                           //  do not use ".RunAsync()" because it will ignore the exception)
        }
    }
}
