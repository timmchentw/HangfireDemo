using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HangfireDemo.Shared.Hosting
{
    public class JobSeriveBase : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;

        private readonly string _executionId;
        private readonly string _jobNamespace;
        private Exception _exception = null;

        public JobSeriveBase(ILogger<JobSeriveBase> logger, IHostApplicationLifetime appLifetime)
        {
            _logger = logger;
            _appLifetime = appLifetime;

            _jobNamespace = GetType().Namespace;    // For distinguish the job
            _executionId = Guid.NewGuid().ToString("N");    // For checking which round
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                ExecuteAsync();
            }
            catch (Exception ex)
            {
                _exception = ex;
                _logger.LogError(ex, $"Job error: {_jobNamespace} ({_executionId})");   // Make sure you have the sink "Console" to identity the error
            }
            finally
            {
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
