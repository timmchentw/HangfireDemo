using Hangfire;
using HangfireDemo.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HangfireDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecurringJobController : Controller
    {
        IWebsiteJobService _websiteJob;

        public RecurringJobController(IWebsiteJobService websiteJob)
        {
            _websiteJob = websiteJob;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult UpsertJob1(string cron)
        {
            string recurringJobName = nameof(IWebsiteJobService.RunJobDemo1);
            RecurringJob.AddOrUpdate(recurringJobName, () => _websiteJob.RunJobDemo1(), cron);
            return new JsonResult("Created: " + recurringJobName);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult UpsertJob1Daily(int hour, int minute)
        {
            string recurringJobName = nameof(IWebsiteJobService.RunJobDemo1);
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
        public IActionResult DeleteJob1(string recurringJobName)
        {
            RecurringJob.RemoveIfExists(recurringJobName);
            return new JsonResult("Deleted: " + recurringJobName);
        }
    }
}
