using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace HangfireDemo.Utils
{
    public enum WebsiteJobEnum
    {
        JobDemo1,
        JobDemo2,
        JobDemo3,
    }

    public enum ConsoleJobEnum
    {
        [Description("ConsoleAppCoreDemo")]
        ConsoleAppCoreDemo,
    }
}
