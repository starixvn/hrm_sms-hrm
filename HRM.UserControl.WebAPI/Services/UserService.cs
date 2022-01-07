namespace HRM.UserControl.WebAPI.Services
{
    using System.Threading.Tasks;
    using HRM.UserControl.Core.Models;
    using HRM.UserControl.Core.Services;
    using HRM.UserControl.Models.User;
    using HRM.UserControl.WebAPI.Criteria.User;

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

        public async Task<CommandResult> LogUserLoginAsync(InsertLogUserLoginCommand command)
        {
            return await baseService.ExecuteNonQueryAsync(command);
        }
    }
}