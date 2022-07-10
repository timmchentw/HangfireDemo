using HangfireDemo.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HangfireDemo.Services
{
    public interface IWebsiteJobService
    {
        void RunJobDemo1();
        void RunJobDemo2WithInput(string input);
        void RunJobDemo3WithNameAndInput(string jobName, string input);
        string EnqueueJobWithEnum(WebsiteJobEnum jobEnum, string[] args = null);
        string ScheduleJobWithEnum(DateTimeOffset scheduleDate, WebsiteJobEnum jobEnum, string[] args = null);
    }
}
