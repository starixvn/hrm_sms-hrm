namespace GO.Website.Services
{
    using System.Threading.Tasks;
    using Core.Services;
    using Criteria.Account;
    using GO.Website.Models.Account;

    public class UserOtpService : IUserOtpService
    {
        private readonly IBaseService baseService;

        public UserOtpService(
            IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<bool> UpdateSecretKeyAsync(int userId, short siteId, string secretKey)
        {
            var criteria = new UpdateSecretKeyCommand
            {
                UserId = userId,
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

        public async Task<UserOtp> GetUserOTPByIdAsync(int userId, short siteId)
        {
            var criteria = new GetUserOTPByIdCriteria { UserId = userId, SiteId = siteId };

            return await baseService.GetDataEntityAsync<UserOtp>(criteria);
        }
    }
}