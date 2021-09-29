namespace HRM.UserControl.Client.Core.Client
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRM.UserControl.Client.Core.Models;
    using HRM.UserControl.Client.Core.Models.Role;

    internal interface IRoleClient
    {
        IList<CompanyRoleFunction> GetUserRoleOnSite();

        Task<IList<CompanyRoleFunction>> GetUserRoleOnSiteAsync();

        CompanyRoleFunction GetUserRoleOnCompany(int companyId);

        Task<CompanyRoleFunction> GetUserRoleOnCompanyAsync(int companyId);

        IList<Company> GetUserCompanyOnCurrentSite();

        Task<IList<Company>> GetUserCompanyOnCurrentSiteAsync();
    }

    internal class RoleClient : BaseHttpClient, IRoleClient
    {
        public IList<Company> GetUserCompanyOnCurrentSite()
        => Get<IList<Company>>(RelativePaths.GetUserComapnyList);

        public async Task<IList<Company>> GetUserCompanyOnCurrentSiteAsync()
            => await GetAsync<IList<Company>>(RelativePaths.GetUserComapnyList);

        public CompanyRoleFunction GetUserRoleOnCompany(int companyId)
        => Get<CompanyRoleFunction>(string.Format(RelativePaths.GetRoleOnCompanyPath, companyId));

        public async Task<CompanyRoleFunction> GetUserRoleOnCompanyAsync(int companyId)
            => await GetAsync<CompanyRoleFunction>(string.Format(RelativePaths.GetRoleOnCompanyPath, companyId));

        public IList<CompanyRoleFunction> GetUserRoleOnSite()
            => Get<IList<CompanyRoleFunction>>(RelativePaths.GetRoleOnSitePath);

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