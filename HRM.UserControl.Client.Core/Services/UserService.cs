namespace HRM.UserControl.Client.Core.Services
{
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Client;
    using HRM.UserControl.Client.Core.Models.User;
    using HRM.UserControl.Client.Core.Services.Interfaces;

    public class UserService : IUserService
    {
        private readonly UserClient userClient;

        public UserService()
        {
            userClient = new UserClient();
        }

        public async Task<User> GetByIdAsync(int userId)
             => await userClient.GetByIdAsync(userId);

        public User GetById(int userId)
            => userClient.GetById(userId);

        public async Task<User> GetByUsernameAsync(string username)
            => await userClient.GetByUsernameAsync(username);

        public User GetByUsername(string username)
            => userClient.GetByUsername(username);
    }
}