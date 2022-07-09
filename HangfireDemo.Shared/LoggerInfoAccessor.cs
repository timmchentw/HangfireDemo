using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog.Sinks.Email;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;

namespace HangfireDemo.Shared
{
    public class LoggerInfoAccessor
    {
        /// <summary>
        /// For getting logger values from configuration
        /// </summary>
        /// <param name="programNamespace">Recommend to use "MethodBase.GetCurrentMethod().DeclaringType.Namespace" to get the namespace from your program/param>
        /// <returns></returns>
        public static Func<HostBuilderContext, LoggerInfo> GetInfo(string programNamespace)
        {
            return (hostContext) =>
            {
                var appInsightsConfig = hostContext.Configuration.GetSection(nameof(Config.ApplicationInsights))
                                            .Get<Config.ApplicationInsights>();
                var emailConfig = hostContext.Configuration.GetSection(nameof(Config.SystemEmailConfig))
                                    .Get<Config.SystemEmailConfig>();

                return new LoggerInfo()
                {
                    AppInsightsIntrumentalKey = appInsightsConfig.InstrumentalKey,
                    EmailSinkInfo = new EmailConnectionInfo()
                    {
                        FromEmail = emailConfig.FromEmail,
                        ToEmail = emailConfig.ToEmail,
                        EmailSubject = $"Log from: {programNamespace}",
                        MailServer = emailConfig.Host,
                        Port = emailConfig.Port,
                        NetworkCredentials = (!string.IsNullOrEmpty(emailConfig.Username) && !string.IsNullOrEmpty(emailConfig.Password)) ?
                        new NetworkCredential()
                        {
                            UserName = emailConfig.Username,
                            Password = emailConfig.Password
                        } : null,
                    },
                    ProgramNamespace = programNamespace,
                    //CanLog = !hostContext.HostingEnvironment.IsDevelopment()  // Not log when "Develop"
                    CanLog = false
                };
            };
        }
    }
}
