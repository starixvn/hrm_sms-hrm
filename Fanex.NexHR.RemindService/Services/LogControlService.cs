// --------------------------------------------------------------------------
// <copyright file="LogControlService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.11.10">Created</change>
// </history>

namespace Fanex.NexHR.ReminderService.Services
{
    using System;
    using System.Data.SqlClient;
    using Fanex.Logging;

    public static class LogControlService
    {
        private const string _TplError = "Service {0} error: {1}";
        private const string _TplInfo = "Service {0}: {1}";
        private const string _TplServiceStart = "Service {0} started at {1}";
        private const string _TplServiceStop = "Service {0} stoped at {1}";
        private const string _TplServicePause = "Service {0} paused at {1}";
        private const string _TplServiceContinue = "Service {0} continued at {1}";
        private const string _TplServiceAfterInstall = "Service {0} installed at {1}";
        private const string _TplServiceAfterUninstall = "Service {0} uninstalled at {1}";
        private const string _TplSqlException = "\r\nProcedure: {0}\r\nLine Number: {1}\r\nServer: {2}";

        public static void WriteException(Exception ex)
        {
            var message = ex.Message;
            SqlException sqlException = ex as SqlException;
            if (sqlException == null && ex.InnerException != null)
            {
                sqlException = ex.InnerException as SqlException;
            }

            if (sqlException != null)
            {
                message += string.Format(_TplSqlException, sqlException.Procedure, sqlException.LineNumber, sqlException.Server);
            }

            LogService.Error(string.Format(_TplError, ConfigSettings.ServiceDisplayName, message));
        }

        public static void WriteError(string error)
        {
            LogService.Error(string.Format(_TplError, ConfigSettings.ServiceDisplayName, error));
        }

        public static void WriteInfo(string info)
        {
            LogService.Info(string.Format(_TplInfo, ConfigSettings.ServiceDisplayName, info));
        }

        public static void WriteBeforeStartingService()
        {
            LogService.Info(string.Format(_TplServiceStart, ConfigSettings.ServiceDisplayName, DateTime.Now));
        }

        public static void WriteAfterStoppingService()
        {
            LogService.Info(string.Format(_TplServiceStop, ConfigSettings.ServiceDisplayName, DateTime.Now));
        }

        public static void WriteWhenPaused()
        {
            LogService.Info(string.Format(_TplServicePause, ConfigSettings.ServiceName, DateTime.Now));
        }

        public static void WhenContinued()
        {
            LogService.Info(string.Format(_TplServiceContinue, ConfigSettings.ServiceDisplayName, DateTime.Now));
        }

        public static void WriteAfterInstall()
        {
            LogService.Info(string.Format(_TplServiceAfterInstall, ConfigSettings.ServiceDisplayName, DateTime.Now));
        }

        public static void WriteAfterUninstall()
        {
            LogService.Info(string.Format(_TplServiceAfterUninstall, ConfigSettings.ServiceDisplayName, DateTime.Now));
        }
    }
}
