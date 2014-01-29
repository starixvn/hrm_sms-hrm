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
        private static bool _allowSendMailToOwnerRequest;
        private static bool _allowSendMailToHR;
        private static bool _allowSendMailToVerifier;
        private static bool _allowSendMailToApprover;
        private static bool _allowSendMailToMailGroup;
        private static bool _isShowCloseBalance = true;

        static ConfigSettings()
        {
            _emailSetting = ConfigurationManager.AppSettings["Email"].ToString();
            _allowSendMailToOwnerRequest = Converter.ToBool(ConfigurationManager.AppSettings["AllowSendMailToOwnerRequest"].ToString());
            _allowSendMailToHR = Converter.ToBool(ConfigurationManager.AppSettings["AllowSendMailToHR"].ToString());
            _allowSendMailToVerifier = Converter.ToBool(ConfigurationManager.AppSettings["AllowSendMailToVerifier"].ToString());
            _allowSendMailToApprover = Converter.ToBool(ConfigurationManager.AppSettings["AllowSendMailToApprover"].ToString());
            _allowSendMailToMailGroup = Converter.ToBool(ConfigurationManager.AppSettings["AllowSendMailToMailGroup"].ToString());
            _isShowCloseBalance = Converter.ToBool(ConfigurationManager.AppSettings["IsCloseBalance"].ToString());
        }

        public static string EmailSetting
        {
            get
            {
                return _emailSetting;
            }
        }

        public static bool AllowSendMailToHR
        {
            get
            {
                return _allowSendMailToHR;
            }
        }

        public static bool AllowSendMailToVerifier
        {
            get
            {
                return _allowSendMailToVerifier;
            }
        }

        public static bool AllowSendMailToApprover
        {
            get
            {
                return _allowSendMailToApprover;
            }
        }

        public static bool AllowSendMailToOwnerRequest
        {
            get
            {
                return _allowSendMailToOwnerRequest;
            }
        }

        public static bool AllowSendMailToMailGroup
        {
            get
            {
                return _allowSendMailToMailGroup;
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
                return Converter.ToBool(ConfigurationManager.AppSettings["AllowEditPersonalInformation"].ToString());
            }
        }
    }
}
