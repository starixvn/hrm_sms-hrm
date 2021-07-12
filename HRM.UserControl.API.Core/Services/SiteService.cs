using System.Threading.Tasks;
using HRM.UserControl.API.Core.Criteria;
using HRM.UserControl.Core.Services;
using HRM.UserControl.Models;

namespace HRM.UserControl.API.Core.Services
{
    public interface ISiteService
    {
        public Task<SiteModel> GetSiteById(int siteId);
    }

    public class SiteService : ISiteService
    {
        private readonly IBaseService baseService;

        public SiteService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<SiteModel> GetSiteById(int siteId)
        {
            var criteria = new GetSiteByIdCriteria
            {
                SiteId = siteId
            };

            return await baseService.GetDataEntityAsync<SiteModel>(criteria);
        }
    }
}