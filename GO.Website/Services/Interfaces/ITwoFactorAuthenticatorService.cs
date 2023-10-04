namespace GO.Website.Services.Interfaces
{
    using GO.Website.Models;

    public interface ITwoFactorAuthenticatorService
    {
        string SetupGoogleAuthInfo(OtpConfig config, string email, string secretKey);

        bool ValidateOTPCode(OtpConfig config, string secretKey, string otpCode);
    }
}