using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;

namespace HangfireDemo.Shared.Hosting
{
    public class JobHost
    {
        public static IHostBuilder CreateJobHostBuilder(string[] args, Func<HostBuilderContext, LoggerInfo> getLoggerInfo)
        {
            return Host.CreateDefaultBuilder(args)
                        .ConfigureHostConfiguration(configurationBuilder =>
                        {
                            configurationBuilder.AddEnvironmentVariables(prefix: "DOTNET_");
                        })
                        .ConfigurSharedAppConfiguration()
                        .ConfigureSerilog(getLoggerInfo);
        }
    }
}
