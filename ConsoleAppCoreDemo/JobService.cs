using HangfireDemo.Shared;
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
        public JobService(ILogger<JobService> logger, IHostApplicationLifetime appLifetime)
            : base(logger, appLifetime)
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
