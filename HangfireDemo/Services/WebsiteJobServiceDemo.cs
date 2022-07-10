using Hangfire;
using HangfireDemo.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HangfireDemo.Services
{
    public class WebsiteJobServiceDemo : IWebsiteJobService
    {
        #region Version without Enum
        public void RunJobDemo1()
        {
            Console.WriteLine("Website job demo 1");
        }

        public void RunJobDemo2WithInput(string input)
        {
            Console.WriteLine("Website job demo 2. Input: " + input);
        }

        [DisplayName("{0}")]    // For displaying job name in dashboard (1st parameter)
        public void RunJobDemo3WithNameAndInput(string jobName, string input)
        {
            Console.WriteLine("Website job demo 3. Input: " + input);
        }
        #endregion


        #region Version with Enum
        public string EnqueueJobWithEnum(WebsiteJobEnum jobEnum, string[] args = null)
        {
            var jobAction = this.GetJobAction(jobEnum, args);
            string jobid = BackgroundJob.Enqueue(jobAction);
            return jobid;
        }

        public string ScheduleJobWithEnum(DateTimeOffset scheduleDate, WebsiteJobEnum jobEnum, string[] args = null)
        {
            var jobAction = this.GetJobAction(jobEnum, args);
            string jobid = BackgroundJob.Schedule(jobAction, scheduleDate);
            return jobid;
        }

        private Expression<Action> GetJobAction(WebsiteJobEnum jobEnum, string[] args)
        {
            switch (jobEnum)
            {
                case WebsiteJobEnum.JobDemo1:
                    return () => this.RunJobDemo1();

                case WebsiteJobEnum.JobDemo2:
                    return () => this.RunJobDemo2WithInput(args[0]);

                case WebsiteJobEnum.JobDemo3:
                    return () => this.RunJobDemo3WithNameAndInput("This is custom job name", args[0]);

                default:
                    throw new ArgumentOutOfRangeException("This job enum is not set");
            }
        }
        #endregion
    }
}
