using System;
using System.Collections.Generic;
using System.Text;

namespace HangfireDemo.Shared.Config
{
    public class SystemEmailConfig
    {
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
