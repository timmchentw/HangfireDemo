using HangfireDemo.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HangfireDemo.Models.Api
{
    public class ConsoleJobScheduleBody
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ConsoleJobEnum jobEnum { get; set; }
        public string[] args { get; set; }
        public DateTimeOffset scheduleDate { get; set; }
    }
}
