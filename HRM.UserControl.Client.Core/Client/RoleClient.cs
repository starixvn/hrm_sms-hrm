namespace HRM.UserControl.Client.Client.Core
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.Role;

    public interface IRoleClient
    {
        Task<IList<CompanyRoleFunction>> GetUserRoleOnSite();

        Task<CompanyRoleFunction> GetUserRoleOnCompany(int companyId);

        Task<IList<Company>> GetUserCompanyOnCurrentSite();
    }

    internal class RoleClient : BaseHttpClient, IRoleClient
    {
        public async Task<IList<Company>> GetUserCompanyOnCurrentSite()
        {
            var companies = await GetAsync<IList<Company>>(RelativePaths.GetUserComapnyList);
            return companies;
        }

        public async Task<CompanyRoleFunction> GetUserRoleOnCompany(int companyId)
        {
            string requestUrl = string.Format(RelativePaths.GetRoleOnCompanyPath, companyId);
            return await GetAsync<CompanyRoleFunction>(requestUrl);
        }

        public async Task<IList<CompanyRoleFunction>> GetUserRoleOnSite()
        {
            return await GetAsync<IList<CompanyRoleFunction>>(RelativePaths.GetRoleOnSitePath);
        }

        private static class RelativePaths
        {
            public const string GetUserComapnyList = "api/user/companies";
            public const string GetRoleOnCompanyPath = "api/user/getRoleOnCompany?companyId={0}";
            public const string GetRoleOnSitePath = "api/user/getRolesOnSite";
        }
    }
}