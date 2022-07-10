using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HangfireDemo.Shared.Hosting
{
    public class JobSeriveBaseWithTelemetry : IHostedService
    {
        protected readonly IHostEnvironment _env;
        protected readonly IConfiguration _config;
        protected readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly TelemetryClient _telemetryClient;

        protected readonly string _executionId;
        protected readonly string _jobNamespace;
        private Exception _exception = null;

        /// <summary>
        /// Hosted service for job. 
        /// You can register this service by "services.AddHostedService<T>()"
        /// Use "IHost.Run()" or "IHost.RunAsync()" to run this service.
        /// The job will terminate the app (Include IHost instance) after run.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="appLifetime"></param>
        /// <param name="telemetryClient">You must register "services.AddApplicationInsightsTelemetryWorkerService()" for injecting this parameter</param>
        public JobSeriveBaseWithTelemetry(IHostEnvironment env, IConfiguration config, ILogger<JobSeriveBaseWithTelemetry> logger, IHostApplicationLifetime appLifetime, TelemetryClient telemetryClient, StartupValues startupValues)
        {
            _env = env;
            _config = config;
            _logger = logger;
            _appLifetime = appLifetime;
            _telemetryClient = telemetryClient;

            _jobNamespace = startupValues.ProgramNamespace;    // For distinguish the job
            _executionId = Guid.NewGuid().ToString("N");    // For checking which round
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                using (_telemetryClient.StartOperation<RequestTelemetry>("My Custom Operation"))    // Track detail information by application insights
                {
                    ExecuteAsync();
                    _telemetryClient.TrackEvent("Log as custom event group");
                }
            }
            catch (Exception ex)
            {
                _exception = ex;
                _logger.LogError(ex, $"Job error: {_jobNamespace} ({_executionId})");   // Make sure you have the sink "Console" to identity the error
            }
            finally
            {
                _telemetryClient.Flush(); // IMPORTNAT! Flush all logs before stopping the app (Or you will lose some logs)
                _appLifetime.StopApplication(); // Go to StopAsync
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            int exitCode = 0;

            if (_exception != null)
            {
                exitCode = 1;
                throw _exception;   // If you want to terminate job with exceptions, throw it to terminate the host. (Non-gracefull termination)
                                    // (You must not use "host.RunAsync()" because the method will ignore the exeption)
                                    // Otherwise, you can stop without throwing exceptions & just call "_logger.LogError(ex)" with "WriteToConsole"
                                    // The calling program will get the console output with exception content
            }

            Environment.ExitCode = exitCode;    // For the calling program to identify that this job had error
            return Task.CompletedTask;
        }


        protected virtual Task ExecuteAsync()
        {
            throw new NotImplementedException("Please override this function for job logic");
        }
    }
}
