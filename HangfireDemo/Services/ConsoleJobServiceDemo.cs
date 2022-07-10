using Hangfire;
using HangfireDemo.Helpers;
using HangfireDemo.Helpers.Hangfire;
using HangfireDemo.Shared;
using HangfireDemo.Utils;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HangfireDemo.Services
{
    public class ConsoleJobServiceDemo : IConsoleJobService
    {
        private readonly IHostEnvironment _env;
        private readonly string _programNamespace;

        public ConsoleJobServiceDemo(IHostEnvironment env, StartupValues startupValues)
        {
            _env = env;
            _programNamespace = startupValues.ProgramNamespace;
        }

        public string EnqueueConsoleJob(ConsoleJobEnum jobEnum, string[] args = null)
        {
            string jobName = this.GetJobName(jobEnum, args);
            string jobId = BackgroundJob.Enqueue(() => this.GetAppPathAndRunProcess(jobName, jobEnum, args));
            HangfireExtension.SetParameterJobName(jobId, jobName);
            return jobId;
        }

        public string ScheduleConsoleJob(DateTimeOffset scheduleDate, ConsoleJobEnum jobEnum, string[] args = null)
        {
            string jobName = this.GetJobName(jobEnum, args);
            string jobId = BackgroundJob.Schedule(() => this.GetAppPathAndRunProcess(jobName, jobEnum, args), scheduleDate);
            HangfireExtension.SetParameterJobName(jobId, jobName);
            return jobId;
        }

        public List<string> RequeueConsoleJob(ConsoleJobEnum jobEnum, string[] args = null)
        {
            string jobName = this.GetJobName(jobEnum, args);
            var jobIds = HangfireExtension.GetJobIds(jobName);
            foreach(string jobId in jobIds)
            {
                BackgroundJob.Requeue(jobId);
            }
            return jobIds;
        }

        public List<string> DeleteConsoleJobsByName(ConsoleJobEnum jobEnum, string[] args = null)
        {
            string jobName = this.GetJobName(jobEnum, args);
            var jobIds = HangfireExtension.GetJobIds(jobName);
            foreach (string jobId in jobIds)
            {
                BackgroundJob.Delete(jobId);
            }
            return jobIds;
        }

        [DisplayName("{0}")]    // For displaying job name in dashboard (1st parameter)
        public void GetAppPathAndRunProcess(string jobName, ConsoleJobEnum jobEnum, string[] args = null)
        {
            string appPath = this.GetConsoleAppPath(jobEnum);
            Util.RunProcess(_env.EnvironmentName, appPath, args);
        }

        private string GetJobName(ConsoleJobEnum jobEnum, string[] args = null)
        {
            string jobProjectName = jobEnum.GetDescription();
            if (args != null && args.Length > 0)
            {
                return $"{jobProjectName} ({string.Join(',',args)})";
            }
            return jobProjectName;
        }

        private string GetConsoleAppPath(ConsoleJobEnum jobEnum)
        {
            string consoleAppDirectory;

            string jobProjectName = jobEnum.GetDescription();

            if (_env.IsDevelopment())
            {
                consoleAppDirectory = this.GetDevProjectAssemblyDirectory(jobProjectName);
            }
            else
            {
                consoleAppDirectory = this.GetProjectAssemblyDirectory(jobProjectName);
            }
            return $@"{consoleAppDirectory}\{jobProjectName}.exe";
        }

        private string GetSolutionRootDirectory(string searchSubDirectory)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var currentDirectory = new DirectoryInfo(currentAssembly.Location).Parent;

            // Search for root directory
            int searchAttempts = 0;
            var searchDirectory = currentDirectory.Parent;
            while (searchAttempts < 10)
            {
                var searchSubDirectories = searchDirectory.GetDirectories();
                if (searchSubDirectories.Any(x => x.Name == searchSubDirectory))
                {
                    return searchDirectory.FullName;
                }
                else
                {
                    searchDirectory = searchDirectory.Parent;   // Search parent directory
                }
            }
            throw new DirectoryNotFoundException("Cannot find program root direcotory");
        }

        private string GetDevProjectAssemblyDirectory(string jobProjectName)
        {
            string devAssemblyDirectory = "";

            //  - root/project/bin/env/...
            string rootDirectory = this.GetSolutionRootDirectory(_programNamespace);
            var assemblyDirectoryCandidate = $@"{rootDirectory}\{jobProjectName}\bin\Debug";
            var subDirectories = Directory.GetDirectories(assemblyDirectoryCandidate);
            if (subDirectories.Length == 0)
            {
                devAssemblyDirectory = assemblyDirectoryCandidate;
            }
            else
            {
                foreach (string subDirectory in subDirectories)
                {
                    var fileNames = Directory.GetFiles($"{subDirectory}");
                    if (fileNames.Any(x => new FileInfo(x).Name.Contains(jobProjectName)))
                    {
                        devAssemblyDirectory = subDirectory;
                        break;
                    }
                }
                if (string.IsNullOrEmpty(devAssemblyDirectory))
                {
                    throw new FileNotFoundException("No dev assembly directory was found");
                }
            }
            return devAssemblyDirectory;
        }

        private string GetProjectAssemblyDirectory(string jobProjectName)
        {
            string assemblyDirectory = "";

            // - env/root/project/yyyyMMdd.N/...
            string rootDirectory = this.GetSolutionRootDirectory(_programNamespace);
            var assemblyDirectoryCandidate = $@"{rootDirectory}\{jobProjectName}";
            var subVersionDirectories = Directory.GetDirectories(assemblyDirectoryCandidate);
            if (subVersionDirectories.Length > 0)
            {
                assemblyDirectory = subVersionDirectories.OrderByDescending(x => x).First();  // Get the lastest version of job project

                var fileNames = Directory.GetFiles($"{assemblyDirectory}");
                if (fileNames.Any(x => new FileInfo(x).Name.Contains(jobProjectName)))
                {
                    return assemblyDirectory;
                }
            }
            throw new FileNotFoundException("No assembly directory is found");
        }
    }
}
