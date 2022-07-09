using Hangfire;
using HangfireDemo.Jobs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HangfireDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BackgroundJobController : Controller
    {
        IWebsiteJob _websiteJob;

        public BackgroundJobController(IWebsiteJob websiteJob)
        {
            _websiteJob = websiteJob;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult EnqueueJob1()
        {
            string jobId = BackgroundJob.Enqueue(() => _websiteJob.RunJobDemo1());
            return new JsonResult("Created. Job ID: " + jobId);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult ScheduleJob1(DateTimeOffset scheduledTime)
        {
            string jobId = BackgroundJob.Schedule(
                            () => _websiteJob.RunJobDemo1(),
                            scheduledTime);
            return new JsonResult("Created. Job ID: " + jobId);
        }

        [HttpDelete]
        [Route("[action]")]
        public IActionResult DeleteJob1(string jobId)
        {
            bool result = BackgroundJob.Delete(jobId);
            if (result) 
                return new JsonResult("Deleted job: " + jobId);
            return StatusCode(500);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult EnqueueJob2(string input)
        {
            string jobId = BackgroundJob.Enqueue(() => _websiteJob.RunJobDemo2WithInput(input));
            return new JsonResult("Created. Job ID: " + jobId);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult EnqueueJob3(string input)
        {
            string jobId = BackgroundJob.Enqueue(() => _websiteJob.RunJobDemo3WithNameAndInput("This is custom job name", input));
            return new JsonResult("Created. Job ID: " + jobId);
        }
    }
}
