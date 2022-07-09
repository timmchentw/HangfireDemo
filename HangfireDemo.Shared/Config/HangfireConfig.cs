using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HangfireDemo.Shared.Config
{
    public class HangfireConfig
    {
        public int ProlongExpirationDays { get; set; }
        public int MaxRetryAttempts { get; set; }
        public int[] DelaysInSeconds { get; set; }
    }
}
