namespace GO.Website.Services
{
    using System;
    using GO.Website.Models;
    using Google.Authenticator;

    public class TwoFactorAuthenticatorService : ITwoFactorAuthenticatorService
    {
        private readonly OtpConfig otpConfig;

        public TwoFactorAuthenticatorService(OtpConfig otpConfig)
        {
            this.otpConfig = otpConfig;
        }

        public string GenerateSetupCode(string email, string secretKey)
        {
            var twoFactorAuthen = new TwoFactorAuthenticator();
            var setupInfo = twoFactorAuthen.GenerateSetupCode(otpConfig.AppName, email, secretKey, false);
            return setupInfo.ManualEntryKey;
        }

        public bool ValidateOTPCode(string secretKey, string otpCode)
        {
            var timeExpireOTP = TimeSpan.FromSeconds(otpConfig.OtpExpireTime);

            var twoFactorAuthenticator = new TwoFactorAuthenticator();

            // document: http://brandonpotter.com/2014/09/07/implementing-free-two-factor-authentication-in-net-using-google-authenticator/
            // accept +/- 5 minute tolerate

            var validOTP = twoFactorAuthenticator.ValidateTwoFactorPIN(secretKey, otpCode, timeExpireOTP);

            return validOTP;
        }
    }
}