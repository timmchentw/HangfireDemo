using Hangfire;
using HangfireDemo.Jobs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HangfireDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecurringJobController : Controller
    {
        IWebsiteJob _websiteJob;

        public RecurringJobController(IWebsiteJob websiteJob)
        {
            _websiteJob = websiteJob;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult UpsertJob1(string cron)
        {
            string recurringJobName = nameof(IWebsiteJob.RunJobDemo1);
            RecurringJob.AddOrUpdate(recurringJobName, () => _websiteJob.RunJobDemo1(), cron);
            return new JsonResult("Created.");
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult UpsertJob1Daily(int hour, int minute)
        {
            string recurringJobName = nameof(IWebsiteJob.RunJobDemo1);
            RecurringJob.AddOrUpdate(recurringJobName, () => _websiteJob.RunJobDemo1(), Cron.Daily(hour, minute));
            return new JsonResult("Created: " + recurringJobName);
        }


        [HttpPut]
        [Route("[action]")]
        public IActionResult TriggerJob1(string recurringJobName)
        {
            RecurringJob.Trigger(recurringJobName);
            return new JsonResult("Triggerd: " + recurringJobName);
        }


        [HttpDelete]
        [Route("[action]")]
        public IActionResult RemoveJob1(string recurringJobName)
        {
            RecurringJob.RemoveIfExists(recurringJobName);
            return new JsonResult("Deleted: " + recurringJobName);
        }
    }
}
