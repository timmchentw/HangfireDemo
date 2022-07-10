using Hangfire;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HangfireDemo.Helpers.Hangfire
{
    public class HangfireExtension
    {
        public static string GetParameterJobName(string jobId, IStorageConnection connection = null)
        {
            var jobConnection = connection ?? JobStorage.Current.GetConnection();

            string parameter = jobConnection.GetJobParameter(jobId, "JobName").Replace(@"""", "");

            if (connection == null) jobConnection.Dispose();
            return parameter;
        }

        public static void SetParameterJobName(string jobId, string jobName, IStorageConnection connection = null)
        {
            var jobConnection = connection ?? JobStorage.Current.GetConnection();

            jobConnection.SetJobParameter(jobId, "JobName", $@"""{jobName}""");

            if (connection == null) jobConnection.Dispose();
        }

        public static List<string> GetJobIds(string jobName, int recentCount = 1000)
        {
            var jobIds = new List<string>();

            var jobApi = JobStorage.Current.GetMonitoringApi();
            using (var jobConnection = JobStorage.Current.GetConnection())
            {
                var queues = jobApi.Queues();
                foreach (var queue in queues)
                {
                    var queueEnqueuedJobs = jobApi.EnqueuedJobs(queue.Name, 0, recentCount)
                                                    .Select(job => job.Key)
                                                    .Where(jobId => GetParameterJobName(jobId, jobConnection) == jobName);

                    var queueFetchedJobs = jobApi.FetchedJobs(queue.Name, 0, recentCount)
                                                    .Select(job => job.Key)
                                                    .Where(jobId => GetParameterJobName(jobId, jobConnection) == jobName);
                    jobIds.AddRange(queueEnqueuedJobs);
                    jobIds.AddRange(queueFetchedJobs);
                }

                var recentScheduledJobs = jobApi.ScheduledJobs(0, recentCount)
                                                .Select(job => job.Key)
                                                .Where(jobId => GetParameterJobName(jobId, jobConnection) == jobName);

                var recentProcessingJobs = jobApi.ProcessingJobs(0, recentCount)
                                                .Select(job => job.Key)
                                                .Where(jobId => GetParameterJobName(jobId, jobConnection) == jobName);

                var recentFailedJobs = jobApi.FailedJobs(0, recentCount)
                                                .Select(job => job.Key)
                                                .Where(jobId => GetParameterJobName(jobId, jobConnection) == jobName);
                jobIds.AddRange(recentScheduledJobs);
                jobIds.AddRange(recentProcessingJobs);
                jobIds.AddRange(recentFailedJobs);
            }

            return jobIds;
        }
    }
}
