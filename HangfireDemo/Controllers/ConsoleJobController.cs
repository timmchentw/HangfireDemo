using Hangfire;
using HangfireDemo.Models.Api;
using HangfireDemo.Services;
using HangfireDemo.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HangfireDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsoleJobController : ControllerBase
    {
        private readonly IConsoleJobService _consoleJobService;

        public ConsoleJobController(IConsoleJobService consoleJobService)
        {
            _consoleJobService = consoleJobService;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Enqueue(ConsoleJobEnqueueBody body)
        {
            string jobId = _consoleJobService.EnqueueConsoleJob(body.jobEnum, body.args);
            return new JsonResult(jobId);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Schedule(ConsoleJobScheduleBody body)
        {  
            string jobId =  _consoleJobService.ScheduleConsoleJob(body.scheduleDate, body.jobEnum, body.args);
            return new JsonResult(jobId);
        }

        [HttpPut]
        [Route("[action]")]
        public IActionResult Requeue(ConsoleJobRequeueBody body)
        {
            var jobIds = _consoleJobService.RequeueConsoleJob(body.jobEnum, body.args);
            return new JsonResult(jobIds.Count > 0 ? jobIds : "");
        }

        [HttpDelete]
        [Route("[action]")]
        public IActionResult Delete(ConsoleJobDeleteBody body)
        {
            var jobIds = _consoleJobService.DeleteConsoleJobsByName(body.jobEnum, body.args);
            return new JsonResult(jobIds.Count > 0 ? jobIds : "");
        }
    }
}
