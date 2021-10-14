namespace HRM.UserControl.Client.Core.Client
{
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Models;

    internal interface IUserClient
    {
        Task<User> GetByUsernameAsync(string email);

        User GetByUsername(string email);

        User GetById(int userId);

        Task<User> GetByIdAsync(int userId);
    }

    internal class UserClient : BaseHttpClient, IUserClient
    {
        public User GetByUsername(string email)
            => Get<User>(string.Format(RelativePaths.GetByUsernamePath, email));

        public async Task<User> GetByUsernameAsync(string email)
            => await GetAsync<User>(string.Format(RelativePaths.GetByUsernamePath, email));

        public User GetById(int userId)
            => Get<User>(string.Format(RelativePaths.GetByIdPath, userId));

        public async Task<User> GetByIdAsync(int userId)
            => await GetAsync<User>(string.Format(RelativePaths.GetByIdPath, userId));

        private static class RelativePaths
        {
            public const string GetByUsernamePath = "api/user/getByUsername?username={0}";
            public const string GetByIdPath = "api/user/{0}";
        }
    }
}