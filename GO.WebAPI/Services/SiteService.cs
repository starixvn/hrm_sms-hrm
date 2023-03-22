namespace GO.WebAPI.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GO.Core.Services;
    using GO.Models;
    using GO.WebAPI.Criteria.Site;

    public class SiteService : ISiteService
    {
        private readonly IBaseService baseService;

        public SiteService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<SiteModel> GetByIdAsync(short siteId)
        {
            var criteria = new GetSiteByIdCriteria
            {
                SiteId = siteId
            };

            return await baseService.GetDataEntityAsync<SiteModel>(criteria);
        }

        public async Task<IEnumerable<SiteModel>> GetByUserIdAsync(int userId, bool? isActive = null)
        {
            var criteria = new GetSiteByUserIdCriteria
            {
                UserId = userId,
                IsActive = isActive
            };
            return await baseService.GetDataAsync<SiteModel>(criteria);
        }
    }
}