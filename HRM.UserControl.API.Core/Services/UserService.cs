namespace HRM.UserControl.API.Core.Services
{
    using System.Threading.Tasks;
    using HRM.UserControl.API.Core.Criteria.User;
    using HRM.UserControl.Core.Models;
    using HRM.UserControl.Core.Services;
    using HRM.UserControl.Models;

    public interface IUserService
    {
        Task<User> GetByUsernameAsync(string username);

        Task<User> GetByIdAsync(int userId);

        Task<CommandResult> UserLoginWriteLogAsync(InsertLogUserLoginCommand command);
    }

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

        public async Task<CommandResult> UserLoginWriteLogAsync(InsertLogUserLoginCommand command)
        {
            return await baseService.ExecuteNonQueryAsync(command);
        }
    }
}