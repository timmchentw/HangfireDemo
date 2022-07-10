using HangfireDemo.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HangfireDemo.Services
{
    public interface IConsoleJobService
    {
        string EnqueueConsoleJob(ConsoleJobEnum jobEnum, string[] args = null);
        string ScheduleConsoleJob(DateTimeOffset scheduleDate, ConsoleJobEnum jobEnum, string[] args = null);
        List<string> RequeueConsoleJob(ConsoleJobEnum jobEnum, string[] args = null);
        List<string> DeleteConsoleJobsByName(ConsoleJobEnum jobEnum, string[] args = null);
    }
}
