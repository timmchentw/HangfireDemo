using HangfireDemo.Shared;
using HangfireDemo.Shared.Hosting;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppCoreDemo
{
    public class JobServiceWithTelemetry : JobSeriveBaseWithTelemetry
    {
        public JobServiceWithTelemetry(IHostEnvironment env, IConfiguration config, ILogger<JobServiceWithTelemetry> logger, IHostApplicationLifetime appLifetime, TelemetryClient telemetryClient, StartupValues startupValues)
            : base(env, config, logger, appLifetime, telemetryClient, startupValues)
        {
            // Init base constructor
        }

        protected override Task ExecuteAsync()
        {
            Console.WriteLine("Sample with app insights");
            return Task.CompletedTask;
        }
    }
}
