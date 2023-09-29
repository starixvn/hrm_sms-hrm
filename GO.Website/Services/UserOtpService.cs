namespace GO.Website.Services
{
    using System.Threading.Tasks;
    using Core.Services;
    using Criteria.Account;
    using Interfaces;

    public class UserOtpService: IUserOtpService
    {
        private readonly IBaseService baseService;
        public UserOtpService(
            IBaseService baseService)
        {
            this.baseService = baseService;
        }
        public async Task<bool> UpdateSecretKeyAsync(int userid, int siteId, string secretKey)
        {
            var criteria = new UpdateSecretKeyCriteria
            {
                UserId = userid,
                SiteId = siteId,
                SecretKey = secretKey
            };

            var result = await baseService.ExecuteNonQueryAsync(criteria);

            return result.IsSuccessful;
        }

        public async Task<bool> UpdateOTPStatusAsync(UpdateStatusUserOTPCommand command)
        {
            var result = await baseService.ExecuteNonQueryAsync(command);

            return result.IsSuccessful;
        }
    }
}
