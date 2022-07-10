using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;

namespace HangfireDemo.Shared.Hosting
{
    public class JobHost
    {
        public static IHostBuilder CreateJobHostBuilder(string[] args, StartupValues startupValues, Func<HostBuilderContext, LoggerInfo> getLoggerInfo)
        {
            return Host.CreateDefaultBuilder(args)
                        .ConfigureHostConfiguration(configurationBuilder =>
                        {
                            configurationBuilder.AddEnvironmentVariables(prefix: "DOTNET_");
                        })
                        .ConfigureSharedAppConfiguration()
                        .ConfigureSerilog(getLoggerInfo)
                        .ConfigureServices((hostContext, services) =>
                        {
                            services.AddSingleton<StartupValues>(startupValues);    // For input some startup values
                        });
        }
    }
}
