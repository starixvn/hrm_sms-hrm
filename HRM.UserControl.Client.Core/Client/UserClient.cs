namespace HRM.UserControl.Client.Core.Client
{
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Models;

    internal interface IUserClient
    {
        Task<User> GetUserByEmailAsync(string email);

        User GetUserByEmail(string email);

        User GetUserById(int userId);

        Task<User> GetUserByIdAsync(int userId);
    }

    internal class UserClient : BaseHttpClient, IUserClient
    {
        public User GetUserByEmail(string email)
            => Get<User>(string.Format(RelativePaths.GetUserByEmailPath, email));

        public async Task<User> GetUserByEmailAsync(string email)
            => await GetAsync<User>(string.Format(RelativePaths.GetUserByEmailPath, email));

        public User GetUserById(int userId)
            => Get<User>(string.Format(RelativePaths.GetUserByIdPath, userId));

        public async Task<User> GetUserByIdAsync(int userId)
            => await GetAsync<User>(string.Format(RelativePaths.GetUserByIdPath, userId));

        private static class RelativePaths
        {
            public const string GetUserByEmailPath = "api/user/getByEmail?email={0}";
            public const string GetUserByIdPath = "api/user/{0}";
        }
    }
}