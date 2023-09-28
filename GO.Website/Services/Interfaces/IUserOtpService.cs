namespace GO.Website.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface IUserOtpService
    {
        Task<bool> UpdateSecretKeyAsync(int userid, int siteId, string secretKey);
        Task<bool> UpdateOTPStatusAsync(int userId, int siteId, bool isOTPRegistered);
    }
}
