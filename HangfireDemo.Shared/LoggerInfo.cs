using Serilog.Sinks.Email;
using System;
using System.Collections.Generic;
using System.Text;

namespace HangfireDemo.Shared
{
    public class LoggerInfo
    {
        public string ProgramNamespace { get; set; }
        public string AppInsightsIntrumentalKey { get; set; }
        public EmailConnectionInfo EmailSinkInfo {get;set;}
    }
}
