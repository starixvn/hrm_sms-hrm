namespace HRM.UserControl.Client.Core.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Client;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.Role;
    using HRM.UserControl.Client.Core.Services.Interfaces;

    public class UserService : IUserService
    {
        private readonly UserClient userClient;
        private readonly RoleClient roleClient;

        public UserService()
        {
            userClient = new UserClient();
            roleClient = new RoleClient();
        }

        public async Task<User> GetByIdAsync(int userId)
             => await userClient.GetUserByIdAsync(userId);

        public User GetById(int userId)
            => userClient.GetUserById(userId);

        public async Task<User> GetByEmailAsync(string email)
            => await userClient.GetUserByEmailAsync(email);

        public User GetByEmail(string email)
            => userClient.GetUserByEmail(email);

        public async Task<IList<Company>> GetCompanyAsync()
             => await roleClient.GetUserCompanyOnCurrentSiteAsync();

        public IList<Company> GetCompany()
            => roleClient.GetUserCompanyOnCurrentSite();

        public async Task<IList<CompanyRoleFunction>> GetPermissionAsync()
            => await roleClient.GetUserRoleOnSiteAsync();

        public IList<CompanyRoleFunction> GetPermission()
             => roleClient.GetUserRoleOnSite();

        public async Task<CompanyRoleFunction> GetPermissionOnCompanyAsync(int companyId)
             => await roleClient.GetUserRoleOnCompanyAsync(companyId);

        public CompanyRoleFunction GetPermissionOnCompany(int companyId)
             => roleClient.GetUserRoleOnCompany(companyId);
    }
}