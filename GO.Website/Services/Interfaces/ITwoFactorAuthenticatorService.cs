namespace GO.Website.Services
{
    public interface ITwoFactorAuthenticatorService
    {
        string GenerateSetupCode(string email, string secretKey);

        bool ValidateOTPCode(string secretKey, string otpCode);
    }
}