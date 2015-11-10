// --------------------------------------------------------------------------
// <copyright file="ConfigSettings.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.11.9">Created</change>
// </history>

namespace Fanex.NexHR.ReminderService
{
    using System.Configuration;
    using Fanex.Common;

    public static class ConfigSettings
    {
        public static string ServiceName
        {
            get
            {
                return ConfigurationManager.AppSettings["ServiceName"].ToString();
            }
        }

        public static string ServiceDisplayName
        {
            get
            {
                var displayName = ConfigurationManager.AppSettings["ServiceDisplayName"] == null 
                    ? ServiceName 
                    : ConfigurationManager.AppSettings["ServiceDisplayName"].ToString();
                return displayName;
            }
        }

        public static string ServiceDescription
        {
            get
            {
                var description = ConfigurationManager.AppSettings["ServiceDescription"] == null
                    ? string.Empty
                    : ConfigurationManager.AppSettings["ServiceDescription"].ToString();
                return description;
            }
        }

        public static int TimerInterval
        {
            get
            {
                return Converter.ToInt(ConfigurationManager.AppSettings["TimerInterval"].ToString());
            }
        }

        public static bool IsAsynchronous
        {
            get
            {
                return Converter.ToBool(ConfigurationManager.AppSettings["IsAsynchronous"].ToString());
            }
        }
    }
}
