namespace GO.Website.Services.Interfaces
{
    using System.Threading.Tasks;
    using Criteria.Account;

    public interface IUserOtpService
    {
        Task<bool> UpdateSecretKeyAsync(int userid, int siteId, string secretKey);
        Task<bool> UpdateOTPStatusAsync(UpdateStatusUserOTPCommand command);
    }
}
