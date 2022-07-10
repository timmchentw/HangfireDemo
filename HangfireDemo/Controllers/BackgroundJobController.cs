using Hangfire;
using HangfireDemo.Models.Api;
using HangfireDemo.Services;
using HangfireDemo.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HangfireDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackgroundJobController : Controller
    {
        IWebsiteJobService _websiteJob;

        public BackgroundJobController(IWebsiteJobService websiteJob)
        {
            _websiteJob = websiteJob;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult ScheduleJob1(DateTimeOffset scheduledTime)
        {
            string jobId = BackgroundJob.Schedule(
                            () => _websiteJob.RunJobDemo1(),
                            scheduledTime);
            return new JsonResult(jobId);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult EnqueueJob1()
        {
            string jobId = BackgroundJob.Enqueue(() => _websiteJob.RunJobDemo1());
            return new JsonResult(jobId);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult EnqueueJob2(string input)
        {
            string jobId = BackgroundJob.Enqueue(() => _websiteJob.RunJobDemo2WithInput(input));
            return new JsonResult(jobId);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult EnqueueJob3(string input)
        {
            string jobId = BackgroundJob.Enqueue(() => _websiteJob.RunJobDemo3WithNameAndInput("This is custom job name", input));
            return new JsonResult(jobId);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult EnqueueJobWithEnum(BackgroundJobEnqueueJobWithEnumBody body)
        {
            string jobId =  _websiteJob.EnqueueJobWithEnum(body.jobEnum, body.args);
            return new JsonResult(jobId);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Requeue(string jobId)
        {
            bool result = BackgroundJob.Requeue(jobId);
            return (result) ? new JsonResult(jobId) : StatusCode(500);
        }

        [HttpDelete]
        [Route("[action]")]
        public IActionResult DeleteJob(string jobId)
        {
            bool result = BackgroundJob.Delete(jobId);
            return (result) ? new JsonResult(jobId) : StatusCode(500);
        }
    }
}
