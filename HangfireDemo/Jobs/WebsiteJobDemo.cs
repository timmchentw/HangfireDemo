using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace HangfireDemo.Jobs
{
    public class WebsiteJobDemo : IWebsiteJob
    {
        public void RunJobDemo1()
        {
            Console.WriteLine("Website job demo 1");
        }

        public void RunJobDemo2WithInput(string input)
        {
            Console.WriteLine("Website job demo 2. Input: " + input);
        }

        [DisplayName("{0}")]    // For displaying job name in dashboard (1st parameter)
        public void RunJobDemo3WithNameAndInput(string jobName, string input)
        {
            Console.WriteLine("Website job demo 3. Input: " + input);
        }
    }
}
