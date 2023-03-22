namespace GO.WebAPI.Services
{
    using System;
    using System.Threading.Tasks;
    using GO.Core.Models;
    using GO.Core.Services;
    using GO.Models.User;
    using GO.WebAPI.Criteria.User;

    public class UserService : IUserService
    {
        private readonly IBaseService baseService;

        public UserService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            var criteria = new GetUserByUsernameCriteria
            {
                Username = username
            };
            return await baseService.GetDataEntityAsync<User>(criteria);
        }

        public async Task<User> GetByIdAsync(int userId)
        {
            var criteria = new GetUserByIdCriteria
            {
                UserId = userId
            };
            return await baseService.GetDataEntityAsync<User>(criteria);
        }

        public async Task<CommandResult> UpdatePasswordAsync(UpdateUserPasswordCommand command)
        {
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> UserCheckInAsync(UserCheckInCommand command)
        {
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> UserReCheckInAsync(UserReCheckInCommand command)
        {
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> UserLogoutAsync(UserLogoutCommand command)
        {
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> SSOCheckInAsync(SSOCheckInCommand command)
        {
            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> LogUserLoginAsync(InsertLogUserLoginCommand command)
        {
            return await baseService.ExecuteNonQueryAsync(command);
        }
    }
}