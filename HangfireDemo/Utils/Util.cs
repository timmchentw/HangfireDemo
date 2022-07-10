using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HangfireDemo.Utils
{
    public class Util
    {
        public static void RunProcess(string environmentName, string filePath, string[] args = null)
        {
            var process = new Process();
            process.StartInfo.FileName = filePath;
            process.StartInfo.Arguments = string.Join(@""" """, args ?? new string[] { });
            process.StartInfo.EnvironmentVariables["DOTNET_ENVIRONMENT"] = environmentName;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;  // Set UseShellExecute to false for redirection.
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;    // Redirect the standard output of the net command. This stream is read asynchronously using an event handler.
            process.StartInfo.RedirectStandardError= true;
            process.EnableRaisingEvents = true; // Raise an event when done.

            string output = "";
            process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                if (!String.IsNullOrEmpty(e.Data))
                {
                    output += e.Data + Environment.NewLine;
                }
            });

            string error = "";
            process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                if (!String.IsNullOrEmpty(e.Data))
                {
                    error += e.Data + Environment.NewLine;
                }
            });

            process.Start();

            // Asynchronously read the standard output of the spawned process.
            // This raises OutputDataReceived events for each line of output.
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();


            int exitCode = process.ExitCode;
            process.Dispose();
            if (exitCode != 0 || !string.IsNullOrEmpty(error))
            {
                throw new ApplicationException($@"The process ""{filePath}"" with args ""{string.Join(',', args)}"" exit and exit code ""{process.ExitCode}"".
Output: {output}
Error: {error}");
            }
        }
    }
}
