namespace GO.Website.Services
{
    using System;
    using GO.Website.Models;
    using GO.Website.Services.Interfaces;
    using Google.Authenticator;

    public class TwoFactorAuthenticatorService : ITwoFactorAuthenticatorService
    {
        public string SetupGoogleAuthInfo(OtpConfig config, string email, string secretKey)
        {
            var twoFactorAuthen = new TwoFactorAuthenticator();
            var setupInfo = twoFactorAuthen.GenerateSetupCode(config.AppName, email, secretKey, false);
            return setupInfo.ManualEntryKey;
        }

        public bool ValidateOTPCode(OtpConfig config, string secretKey, string otpCode)
        {
            var timeExpireOTP = TimeSpan.FromSeconds(config.OtpExpireTime);

            var twoFactorAuthenticator = new TwoFactorAuthenticator();

            // document: http://brandonpotter.com/2014/09/07/implementing-free-two-factor-authentication-in-net-using-google-authenticator/
            // accept +/- 5 minute tolerate

            var validOTP = twoFactorAuthenticator.ValidateTwoFactorPIN(secretKey, otpCode, timeExpireOTP);

            return validOTP;
        }
    }
}