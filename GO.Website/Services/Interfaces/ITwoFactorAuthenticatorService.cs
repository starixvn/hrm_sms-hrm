using GO.Website.Models;

namespace GO.Website.Services.Interfaces
{
    public interface ITwoFactorAuthenticatorService
    {
        string SetupGoogleAuthInfo(OtpConfig config, string username, string secretKey);
        bool ValidateOTPCode(OtpConfig config, string secretKey, string otpCode);
    }
}
