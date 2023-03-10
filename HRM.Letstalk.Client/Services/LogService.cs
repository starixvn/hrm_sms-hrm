using Newtonsoft.Json;
using NLog.Web;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using NLog;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using Fanex.Data.Repository;

namespace HRM.Letstalk.Client.Services
{
    public class LogService
    {
        public interface ILetstalkLogService
        {
            void ErrorAsync(Exception exception);

            void ErrorAsync(Exception exception, ICriteria criteria);

            void LogInformation(string message);
        }

        public class LetstalkLogService : ILetstalkLogService
        {
            private readonly Logger nlog = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

            public void ErrorAsync(Exception exception)
            {
                var logMessage = new LogMessage(exception.Message)
                           .AddSection("Database Info", DatabaseInfoGetters.GetDbExceptionInfo(exception))
                           .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                           .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
                           .ToString();
                nlog.Error(logMessage);
            }

            public void ErrorAsync(Exception exception, ICriteria criteria)
            {
                var logMessage = new LogMessage(exception.Message)
                          .AddSection("Database Info", DatabaseInfoGetters.GetDbExceptionInfo(exception))
                          .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                          .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
                          .AddMessage(JsonConvert.SerializeObject(criteria, Formatting.Indented))
                          .ToString();
                 nlog.Error(logMessage);
            }

            public void LogInformation(string message)
            {
                nlog.Info(message);
            }
        }
    }
}
