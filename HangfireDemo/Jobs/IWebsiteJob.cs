using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HangfireDemo.Jobs
{
    public interface IWebsiteJob
    {
        void RunJobDemo1();
        void RunJobDemo2WithInput(string id);
        void RunJobDemo3WithNameAndInput(string jobName, string id);
    }
}
