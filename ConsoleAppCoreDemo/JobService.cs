using HangfireDemo.Shared;
using HangfireDemo.Shared.Hosting;
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
    public class JobService : JobSeriveBase
    {
        public JobService(IHostEnvironment env, IConfiguration config, ILogger<JobService> logger, IHostApplicationLifetime appLifetime, StartupValues startupValues)
            : base(env, config, logger, appLifetime, startupValues)
        {
            // Init base constructor
        }

        protected override Task ExecuteAsync()
        {
            Console.WriteLine("Sample");
            return Task.CompletedTask;
        }
    }
}
