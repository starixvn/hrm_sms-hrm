namespace HRM.UserControl.Client.Core.Client
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.Role;

    internal interface IRoleClient
    {
        Task<IList<CompanyRoleFunction>> GetUserRoleOnSiteAsync();

        Task<CompanyRoleFunction> GetUserRoleOnCompanyAsync(int companyId);

        Task<IList<Company>> GetUserCompanyOnCurrentSiteAsync();
    }

    internal class RoleClient : BaseHttpClient, IRoleClient
    {
        public async Task<IList<Company>> GetUserCompanyOnCurrentSiteAsync()
            => await GetAsync<IList<Company>>(RelativePaths.GetUserComapnyList);

        public async Task<CompanyRoleFunction> GetUserRoleOnCompanyAsync(int companyId)
            => await GetAsync<CompanyRoleFunction>(string.Format(RelativePaths.GetRoleOnCompanyPath, companyId));

        public async Task<IList<CompanyRoleFunction>> GetUserRoleOnSiteAsync()
            => await GetAsync<IList<CompanyRoleFunction>>(RelativePaths.GetRoleOnSitePath);

        private static class RelativePaths
        {
            public const string GetUserComapnyList = "api/user/companies";
            public const string GetRoleOnCompanyPath = "api/user/getRoleOnCompany?companyId={0}";
            public const string GetRoleOnSitePath = "api/user/getRolesOnSite";
        }
    }
}