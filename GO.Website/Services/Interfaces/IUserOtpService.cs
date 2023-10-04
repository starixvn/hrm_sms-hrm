namespace GO.Website.Services.Interfaces
{
    using System.Threading.Tasks;
    using Criteria.Account;
    using Models.Account;

    public interface IUserOtpService
    {
        Task<bool> UpdateSecretKeyAsync(int userId, int siteId, string secretKey);
        Task<bool> UpdateOTPStatusAsync(UpdateStatusUserOTPCommand command);
        Task<UserOtp> GetUserOTPById(int userId, int siteId);
    }
}
