using HangfireDemo.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace ConsoleAppCoreDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Util.CreateJobHostBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<JobService>();    // Register job service
                })
                .Build()
                .RunAsync();    // Run logic in job service
        }
    }
}
