using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HangfireDemo.Shared
{
    public class JobSeriveBase : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly string _executionId;
        private readonly string _jobNamespace;
        private string _errorTitle => $"Error from job: {_jobNamespace}";

        public JobSeriveBase(ILogger<JobSeriveBase> logger, IHostApplicationLifetime appLifetime)
        {
            _logger = logger;
            _appLifetime = appLifetime;

            _jobNamespace = GetType().Namespace;
            _executionId = Guid.NewGuid().ToString("N");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                this.ExecuteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{_errorTitle} ({_executionId})");
                throw;  // 讓Hangfire顯示Job錯誤內容，因此必須拋出Exception (如無，則須注意Exit code須設為-1)
            }
            _appLifetime.StopApplication(); // Go to StopAsync

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Do something when ending the job
            Environment.ExitCode = 1;   // 如要考慮Exception狀態，須考慮有-1的狀態
            return Task.CompletedTask;
        }


        protected virtual Task ExecuteAsync()
        {
            throw new NotImplementedException("Please override this function for job logic");
        }
    }
}
