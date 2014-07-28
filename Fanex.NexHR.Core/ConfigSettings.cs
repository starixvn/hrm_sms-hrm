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
    public class ConfigSettings
    {
        private static string _emailSetting;
        private static bool _isShowCloseBalance;
        private static bool _allowEditPersonalInformation;
        private static bool _allowAdminLogin;
        
        static ConfigSettings()
        {
            _emailSetting = ConfigurationManager.AppSettings["Email"].ToString();
            _isShowCloseBalance = Converter.ToBool(ConfigurationManager.AppSettings["IsCloseBalance"].ToString());
            _allowEditPersonalInformation = Converter.ToBool(ConfigurationManager.AppSettings["AllowEditPersonalInformation"].ToString());
            _allowAdminLogin = Converter.ToBool(ConfigurationManager.AppSettings["AllowAdminLogin"].ToString());
        }

        public static string EmailSetting
        {
            get
            {
                return _emailSetting;
            }
        }

        public static bool IsShowCloseBalance
        {
            get
            {
                return _isShowCloseBalance;
            }
        }

        public static bool AllowEditPersonalInformation
        {
            get
            {
                return _allowEditPersonalInformation;
            }
        }

        public static bool AllowAdminLogin
        {
            get
            {
                return _allowAdminLogin;
            }
        }
    }
}
