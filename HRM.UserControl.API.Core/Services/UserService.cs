namespace HRM.UserControl.API.Core.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRM.UserControl.API.Core.Criteria;
    using HRM.UserControl.API.Core.Models.Role;
    using HRM.UserControl.API.Core.Services.Interfaces;
    using HRM.UserControl.Core.Models;
    using HRM.UserControl.Core.Services;
    using HRM.UserControl.Models;

    public class UserService : IUserService
    {
        private readonly IBaseService baseService;

        public UserService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var criteria = new GetUserByEmailCriteria
            {
                Email = email
            };

            return await baseService.GetDataEntityAsync<User>(criteria);
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            var criteria = new GetUserByIdCriteria
            {
                UserId = userId
            };

            return await baseService.GetDataEntityAsync<User>(criteria);
        }

        public async Task<IEnumerable<Company>> GetUserCompanyBySiteAsync(int userId, int siteId)
        {
            var criteria = new GetUserCompanyBySiteCriteria
            {
                UserId = userId,
                SiteId = siteId,
            };
            return await baseService.GetDataAsync<Company>(criteria);
        }

        public async Task<IEnumerable<SiteModel>> GetUserSiteCompanyAsync(int userId)
        {
            var criteria = new GetUserSiteCompanyByUserIdCriteria
            {
                UserId = userId,
                IsActive = true
            };

            var siteCompanies = await baseService.GetDataAsync<UserSiteCompany>(criteria);

            var sites = MappingSietCompany(siteCompanies);

            return sites;
        }

        public async Task<CommandResult> UserLoginWriteLogAsync(InsertUserLoginLogCommand command)
        {
            return await baseService.ExecuteNonQueryAsync(command);
        }

        private static IEnumerable<SiteModel> MappingSietCompany(IEnumerable<UserSiteCompany> userSiteCompanies)
        {
            var sites = userSiteCompanies.GroupBy(usc => usc.SiteId).Select(g => new SiteModel
            {
                SiteId = g.Key,
                SiteName = g.FirstOrDefault(x => x.SiteId == g.Key)?.SiteName,
                AccessToken = g.FirstOrDefault(x => x.SiteId == g.Key)?.AccessToken,
                IsActive = g.FirstOrDefault(x => x.SiteId == g.Key).IsActive,
            });

            foreach (var site in sites)
            {
                site.CompanyList = userSiteCompanies.Where(i => i.SiteId == site.SiteId)
                    .Select(x => new Company
                    {
                        CompanyId = x.CompanyId,
                        Name = x.CompanyName
                    });
            }

            return sites;
        }
    }
}