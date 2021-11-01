namespace HRM.UserControl.Client.Core.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Client;
    using HRM.UserControl.Client.Core.Models;

    public interface IUserService
    {
        IList<MenuItem> GetMenuItem();

        Task<IList<MenuItem>> GetMenuItemAsync();

        IList<FunctionPermission> GetFunctionPermission();

        Task<IList<FunctionPermission>> GetFunctionPermissionAsync();
    }

    public class UserService : IUserService
    {
        private readonly UserClient userClient;

        public UserService()
        {
            userClient = new UserClient();
        }

        public async Task<IList<MenuItem>> GetMenuItemAsync()
             => await userClient.GetMenuItemAsync();

        public IList<MenuItem> GetMenuItem()
            => userClient.GetMenuItem();

        public async Task<IList<FunctionPermission>> GetFunctionPermissionAsync()
             => await userClient.GetFunctionPermissionAsync();

        public IList<FunctionPermission> GetFunctionPermission()
            => userClient.GetFunctionPermission();
    }
}