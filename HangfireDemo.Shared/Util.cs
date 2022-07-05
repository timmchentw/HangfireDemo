using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Email;
using System;
using System.IO;
using System.Reflection;

namespace HangfireDemo.Shared
{
    public class Util
    {
        public static IHostBuilder CreateJobHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                        .ConfigureHostConfiguration(configurationBuilder =>
                        {
                            configurationBuilder.AddEnvironmentVariables(prefix: "DOTNET_");
                        })
                        .ConfigureAppConfiguration((hostContext, config) =>
                        {
                            var env = hostContext.HostingEnvironment;
                            string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                            config = config.SetBasePath(assemblyDirectory)
                                            .AddJsonFile("sharedsettings.json", optional: false, reloadOnChange: true)
                                            .AddJsonFile($"sharedsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                                            .AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
                                            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                        })
                        .UseSerilog((hostContext, loggerConfig) =>
                        {
                            string jobNamespace = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
                            var env = hostContext.HostingEnvironment;
                            var emailSinkInfo = new EmailConnectionInfo() { /*Assign Properties...*/ };

                            loggerConfig.MinimumLevel.Information()
                            .Enrich.WithProperty("Application", jobNamespace)
                            .Enrich.WithProperty("Environment", hostContext.HostingEnvironment.EnvironmentName)
                            .WriteTo.Conditional(
                                condition => !env.IsDevelopment(),
                                writeTo => writeTo.Console()
                                           /*.WriteTo.ApplicationInsights(new TelemetryConfiguration("Assign AppInsights Key..."), TelemetryConverter.Traces)
                                           .WriteTo.Email(emailSinkInfo, restrictedToMinimumLevel: LogEventLevel.Error)*/);
                        });
        }
    }
}
