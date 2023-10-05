namespace GO.Website.Services
{
    using System.Threading.Tasks;
    using Criteria.Account;
    using Models.Account;

    public interface IUserOtpService
    {
        Task<bool> UpdateSecretKeyAsync(int userId, short siteId, string secretKey);

        Task<bool> UpdateOTPStatusAsync(UpdateStatusUserOTPCommand command);

        Task<UserOtp> GetUserOTPByIdAsync(int userId, short siteId);
    }
}