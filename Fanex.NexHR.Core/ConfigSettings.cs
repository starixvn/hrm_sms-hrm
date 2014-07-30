// --------------------------------------------------------------------------
// <copyright file="ConfigSettings.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.11.14">Created</change>
// </history>

namespace Fanex.NexHR.Core
{
    using System.Configuration;
    using Fanex.Common;

    /// <summary>
    /// Config Settings.
    /// </summary>
    public static class ConfigSettings
    {
        public static string EmailSetting
        {
            get
            {
                return ConfigurationManager.AppSettings["Email"].ToString();
            }
        }

        public static bool IsShowCloseBalance
        {
            get
            {
                return Converter.ToBool(ConfigurationManager.AppSettings["IsCloseBalance"].ToString());
            }
        }

        public static bool AllowEditPersonalInformation
        {
            get
            {
                return Converter.ToBool(ConfigurationManager.AppSettings["AllowEditPersonalInformation"].ToString());
            }
        }

        public static bool AllowAdminLogin
        {
            get
            {
                return Converter.ToBool(ConfigurationManager.AppSettings["AllowAdminLogin"].ToString());
            }
        }
    }
}
