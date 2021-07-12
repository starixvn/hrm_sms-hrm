namespace HRM.UserControl.Client.Core.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Client.Core;
    using HRM.UserControl.Client.Core.Client.Core;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.Auth;
    using HRM.UserControl.Client.Core.Models.Role;
    using HRM.UserControl.Client.Core.Services.Interfaces;

    public class UserService : IUserService
    {
        private readonly UserClient userClient = new UserClient();
        private readonly RoleClient roleClient = new RoleClient();

        public async Task<User> GetAsync(int userId)
        {
            var user = await userClient.GetUserByIdAsync(userId);
            return user;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var user = await userClient.GetUserByEmailAsync(email);
            return user;
        }

        public async Task<IList<Company>> GetCompanyAsync()
        {
            var companies = await roleClient.GetUserCompanyOnCurrentSite();
            return companies;
        }

        public async Task<IList<CompanyRoleFunction>> GetPermissionAsync()
        {
            var permissions = await roleClient.GetUserRoleOnSite();
            return permissions;
        }

        public async Task<CompanyRoleFunction> GetPermissionOnCompanyAsync(int companyId)
        {
            var permission = await roleClient.GetUserRoleOnCompany(companyId);
            return permission;
        }

        public async Task<LoginResult> LoginAsync(LoginParam loginParam)
        {
            var result = await userClient.LoginAsync(loginParam);
            return result;
        }
    }
}